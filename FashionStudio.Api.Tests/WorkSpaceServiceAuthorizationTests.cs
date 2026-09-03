using FashionStudio.Api.Exceptions;
using FashionStudio.Api.Models;
using FashionStudio.Api.Services;
using Xunit;

namespace FashionStudio.Api.Tests
{
    // OrderService, FittingService, and PaymentService all delegate their
    // workspace-role checks here — these two methods are the single source
    // of truth for "can this user do this" across every feature.
    public class WorkSpaceServiceAuthorizationTests
    {
        private static async Task<WorkSpace> SeedWorkSpaceAsync(
            Data.AppDbContext context, int userId, Role? role)
        {
            var workSpace = new WorkSpace { Name = "WS" };
            context.WorkSpaces.Add(workSpace);
            await context.SaveChangesAsync();

            if (role != null)
            {
                context.WorkSpaceMemberships.Add(new WorkSpaceMembership { UserId = userId, WorkSpaceId = workSpace.Id, Role = role });
                await context.SaveChangesAsync();
            }

            return workSpace;
        }

        [Theory]
        [InlineData(Role.Owner)]
        [InlineData(Role.Assistant)]
        public async Task EnsureIsOwnerOrAssistantAsync_AllowsOwnerAndAssistant(Role role)
        {
            using var context = TestHelpers.CreateContext();
            var workSpace = await SeedWorkSpaceAsync(context, userId: 1, role);
            var service = new WorkSpaceService(context, TestHelpers.CreateMapper(), new NotImplementedUserService());

            // Should not throw.
            await service.EnsureIsOwnerOrAssistantAsync(workSpace.Id, userId: 1, CancellationToken.None);
        }

        [Fact]
        public async Task EnsureIsOwnerOrAssistantAsync_RejectsTailor()
        {
            using var context = TestHelpers.CreateContext();
            var workSpace = await SeedWorkSpaceAsync(context, userId: 1, Role.Tailor);
            var service = new WorkSpaceService(context, TestHelpers.CreateMapper(), new NotImplementedUserService());

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                service.EnsureIsOwnerOrAssistantAsync(workSpace.Id, userId: 1, CancellationToken.None));
        }

        [Fact]
        public async Task EnsureIsOwnerOrAssistantAsync_RejectsNonMember()
        {
            using var context = TestHelpers.CreateContext();
            var workSpace = await SeedWorkSpaceAsync(context, userId: 1, role: null);
            var service = new WorkSpaceService(context, TestHelpers.CreateMapper(), new NotImplementedUserService());

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                service.EnsureIsOwnerOrAssistantAsync(workSpace.Id, userId: 1, CancellationToken.None));
        }

        [Fact]
        public async Task EnsureIsMemberAsync_AllowsAnyRole()
        {
            using var context = TestHelpers.CreateContext();
            var workSpace = await SeedWorkSpaceAsync(context, userId: 1, Role.Tailor);
            var service = new WorkSpaceService(context, TestHelpers.CreateMapper(), new NotImplementedUserService());

            await service.EnsureIsMemberAsync(workSpace.Id, userId: 1, CancellationToken.None);
        }

        [Fact]
        public async Task EnsureIsMemberAsync_RejectsNonMember()
        {
            using var context = TestHelpers.CreateContext();
            var workSpace = await SeedWorkSpaceAsync(context, userId: 1, role: null);
            var service = new WorkSpaceService(context, TestHelpers.CreateMapper(), new NotImplementedUserService());

            await Assert.ThrowsAsync<NotFoundException>(() =>
                service.EnsureIsMemberAsync(workSpace.Id, userId: 1, CancellationToken.None));
        }
    }
}
