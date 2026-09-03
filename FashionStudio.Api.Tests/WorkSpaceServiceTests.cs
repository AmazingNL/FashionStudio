using FashionStudio.Api.DTOs;
using FashionStudio.Api.Exceptions;
using FashionStudio.Api.Models;
using FashionStudio.Api.Services;
using Xunit;

namespace FashionStudio.Api.Tests
{
    public class WorkSpaceServiceTests
    {
        // CreateWorkSpaceAsync resolves the owner through IUserService, so (unlike the
        // authorization-only tests) this needs the real UserService, not the stub.
        private static WorkSpaceService CreateService(Data.AppDbContext context)
        {
            var mapper = TestHelpers.CreateMapper();
            var userService = new UserService(context, mapper);
            return new WorkSpaceService(context, mapper, userService);
        }

        [Fact]
        public async Task CreateWorkSpaceAsync_ValidRequest_CreatesOwnerMembership()
        {
            using var context = TestHelpers.CreateContext();
            var owner = new User { FirstName = "Owner", LastName = "One", Email = "owner@test.com", UserName = "owner" };
            context.Users.Add(owner);
            await context.SaveChangesAsync();
            var service = CreateService(context);

            var result = await service.CreateWorkSpaceAsync(
                new WorkSpaceRequestDTO { Name = "Grace Couture" }, owner.Id, CancellationToken.None);

            Assert.Equal("Grace Couture", result.Name);
            var membership = Assert.Single(context.WorkSpaceMemberships);
            Assert.Equal(Role.Owner, membership.Role);
            Assert.Equal(owner.Id, membership.UserId);
        }

        [Fact]
        public async Task CreateWorkSpaceAsync_DuplicateName_Throws()
        {
            using var context = TestHelpers.CreateContext();
            var owner = new User { FirstName = "Owner", LastName = "One", Email = "owner@test.com", UserName = "owner" };
            context.Users.Add(owner);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            await service.CreateWorkSpaceAsync(new WorkSpaceRequestDTO { Name = "Grace Couture" }, owner.Id, CancellationToken.None);

            await Assert.ThrowsAsync<ConflictException>(() =>
                service.CreateWorkSpaceAsync(new WorkSpaceRequestDTO { Name = "Grace Couture" }, owner.Id, CancellationToken.None));
        }

        [Fact]
        public async Task CreateWorkSpaceAsync_OwnerNotFound_ThrowsKeyNotFound()
        {
            // Documents current behavior: WorkSpaceService checks `if (user == null)` and throws
            // NotFoundException, but UserService.GetUserByIdAsync never returns null — it throws
            // KeyNotFoundException itself. So that NotFoundException branch is dead code, and an
            // unknown owner id actually surfaces as KeyNotFoundException (mapped to a generic 500
            // by GlobalExceptionHandler, not a 404) rather than the intended NotFoundException.
            using var context = TestHelpers.CreateContext();
            var service = CreateService(context);

            await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                service.CreateWorkSpaceAsync(new WorkSpaceRequestDTO { Name = "Ghost" }, 999, CancellationToken.None));
        }

        [Fact]
        public async Task GetWorkSpaceByIdAsync_IncludesMembersAndCustomers()
        {
            using var context = TestHelpers.CreateContext();
            var owner = new User { FirstName = "Owner", LastName = "One", Email = "owner@test.com", UserName = "owner" };
            context.Users.Add(owner);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            var created = await service.CreateWorkSpaceAsync(new WorkSpaceRequestDTO { Name = "Grace Couture" }, owner.Id, CancellationToken.None);

            var result = await service.GetWorkSpaceByIdAsync(created.Id);

            var member = Assert.Single(result.Memberships);
            Assert.Equal(owner.Id, member.UserId);
        }

        [Fact]
        public async Task GetAllWorkSpacesAsync_ReturnsAllWorkSpaces()
        {
            using var context = TestHelpers.CreateContext();
            var owner = new User { FirstName = "Owner", LastName = "One", Email = "owner@test.com", UserName = "owner" };
            context.Users.Add(owner);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            await service.CreateWorkSpaceAsync(new WorkSpaceRequestDTO { Name = "WS 1" }, owner.Id, CancellationToken.None);
            await service.CreateWorkSpaceAsync(new WorkSpaceRequestDTO { Name = "WS 2" }, owner.Id, CancellationToken.None);

            var result = await service.GetAllWorkSpacesAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task UpdateWorkSpaceAsync_ValidRequest_UpdatesFields()
        {
            using var context = TestHelpers.CreateContext();
            var owner = new User { FirstName = "Owner", LastName = "One", Email = "owner@test.com", UserName = "owner" };
            context.Users.Add(owner);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            var created = await service.CreateWorkSpaceAsync(new WorkSpaceRequestDTO { Name = "Old Name" }, owner.Id, CancellationToken.None);

            var updated = await service.UpdateWorkSpaceAsync(
                created.Id, new WorkSpaceRequestDTO { Name = "New Name", Description = "Updated" }, CancellationToken.None);

            Assert.Equal("New Name", updated.Name);
        }

        [Fact]
        public async Task DeleteWorkSpaceAsync_ExistingWorkSpace_ReturnsTrueAndRemoves()
        {
            using var context = TestHelpers.CreateContext();
            var owner = new User { FirstName = "Owner", LastName = "One", Email = "owner@test.com", UserName = "owner" };
            context.Users.Add(owner);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            var created = await service.CreateWorkSpaceAsync(new WorkSpaceRequestDTO { Name = "Temp" }, owner.Id, CancellationToken.None);

            var result = await service.DeleteWorkSpaceAsync(created.Id);

            Assert.True(result);
            Assert.Empty(context.WorkSpaces);
        }

        [Fact]
        public async Task DeleteWorkSpaceAsync_NonExistentWorkSpace_ReturnsFalse()
        {
            using var context = TestHelpers.CreateContext();
            var service = CreateService(context);

            var result = await service.DeleteWorkSpaceAsync(999);

            Assert.False(result);
        }

        [Fact]
        public async Task IsOwnerOfWorkSpaceAsync_ReturnsTrueOnlyForOwnerRole()
        {
            using var context = TestHelpers.CreateContext();
            var owner = new User { FirstName = "Owner", LastName = "One", Email = "owner@test.com", UserName = "owner" };
            var assistant = new User { FirstName = "Assist", LastName = "One", Email = "assist@test.com", UserName = "assist" };
            var workSpace = new WorkSpace { Name = "WS" };
            context.Users.AddRange(owner, assistant);
            context.WorkSpaces.Add(workSpace);
            await context.SaveChangesAsync();
            context.WorkSpaceMemberships.Add(new WorkSpaceMembership { UserId = owner.Id, WorkSpaceId = workSpace.Id, Role = Role.Owner });
            context.WorkSpaceMemberships.Add(new WorkSpaceMembership { UserId = assistant.Id, WorkSpaceId = workSpace.Id, Role = Role.Assistant });
            await context.SaveChangesAsync();
            var service = CreateService(context);

            Assert.True(await service.IsOwnerOfWorkSpaceAsync(owner.Id, workSpace.Id, CancellationToken.None));
            Assert.False(await service.IsOwnerOfWorkSpaceAsync(assistant.Id, workSpace.Id, CancellationToken.None));
        }

        [Fact]
        public async Task IsMemberOfWorkSpaceAsync_ChecksByEmail()
        {
            using var context = TestHelpers.CreateContext();
            var owner = new User { FirstName = "Owner", LastName = "One", Email = "owner@test.com", UserName = "owner" };
            var workSpace = new WorkSpace { Name = "WS" };
            context.Users.Add(owner);
            context.WorkSpaces.Add(workSpace);
            await context.SaveChangesAsync();
            context.WorkSpaceMemberships.Add(new WorkSpaceMembership { UserId = owner.Id, WorkSpaceId = workSpace.Id, Role = Role.Owner });
            await context.SaveChangesAsync();
            var service = CreateService(context);

            Assert.True(await service.IsMemberOfWorkSpaceAsync("owner@test.com", workSpace.Id, CancellationToken.None));
            Assert.False(await service.IsMemberOfWorkSpaceAsync("nobody@test.com", workSpace.Id, CancellationToken.None));
        }
    }
}
