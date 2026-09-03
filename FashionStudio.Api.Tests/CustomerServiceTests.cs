using FashionStudio.Api.DTOs;
using FashionStudio.Api.Exceptions;
using FashionStudio.Api.Models;
using FashionStudio.Api.Services;
using Xunit;

namespace FashionStudio.Api.Tests
{
    public class CustomerServiceTests
    {
        private static CustomerService CreateService(Data.AppDbContext context)
        {
            var mapper = TestHelpers.CreateMapper();
            var workSpaceService = new WorkSpaceService(context, mapper, new NotImplementedUserService());
            return new CustomerService(new NotImplementedUserService(), workSpaceService, mapper, context);
        }

        [Fact]
        public async Task CreateCustomerAsync_ValidRequest_Succeeds()
        {
            using var context = TestHelpers.CreateContext();
            var user = new User { FirstName = "Owner", LastName = "One", Email = "owner@test.com", UserName = "owner" };
            context.Users.Add(user);
            await context.SaveChangesAsync();
            var service = CreateService(context);

            var result = await service.CreateCustomerAsync(
                new CustomerRequestDTO { FullName = "Amara Okafor", Phone = "555-0001" }, user.Id, CancellationToken.None);

            Assert.Equal("Amara Okafor", result.FullName);
            Assert.Null(result.WorkSpaceId);
        }

        [Fact]
        public async Task CreateCustomerAsync_DuplicatePhone_Throws()
        {
            using var context = TestHelpers.CreateContext();
            var user = new User { FirstName = "Owner", LastName = "One", Email = "owner@test.com", UserName = "owner" };
            context.Users.Add(user);
            await context.SaveChangesAsync();
            var service = CreateService(context);

            await service.CreateCustomerAsync(
                new CustomerRequestDTO { FullName = "First Customer", Phone = "555-0001" }, user.Id, CancellationToken.None);

            await Assert.ThrowsAsync<ConflictException>(() =>
                service.CreateCustomerAsync(
                    new CustomerRequestDTO { FullName = "Second Customer", Phone = "555-0001" }, user.Id, CancellationToken.None));
        }

        [Fact]
        public async Task CreateCustomerAsync_CreatingUserNotFound_Throws()
        {
            using var context = TestHelpers.CreateContext();
            var service = CreateService(context);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                service.CreateCustomerAsync(
                    new CustomerRequestDTO { FullName = "Amara Okafor", Phone = "555-0001" }, 999, CancellationToken.None));
        }

        [Fact]
        public async Task AssignCustomerToWorkSpaceAsync_OwnerCaller_Succeeds()
        {
            using var context = TestHelpers.CreateContext();
            var owner = new User { FirstName = "Owner", LastName = "One", Email = "owner@test.com", UserName = "owner" };
            var customer = new Customer { FullName = "Amara Okafor", Phone = "555-0001" };
            var workSpace = new WorkSpace { Name = "WS" };
            context.Users.Add(owner);
            context.Customers.Add(customer);
            context.WorkSpaces.Add(workSpace);
            await context.SaveChangesAsync();
            context.WorkSpaceMemberships.Add(new WorkSpaceMembership { UserId = owner.Id, WorkSpaceId = workSpace.Id, Role = Role.Owner });
            await context.SaveChangesAsync();
            var service = CreateService(context);

            var result = await service.AssignCustomerToWorkSpaceAsync(customer.Id, workSpace.Id, owner.Id, CancellationToken.None);

            Assert.Equal(workSpace.Id, result.WorkSpaceId);
        }

        [Fact]
        public async Task AssignCustomerToWorkSpaceAsync_CallerNotOwnerOrAssistant_Throws()
        {
            using var context = TestHelpers.CreateContext();
            var tailor = new User { FirstName = "Tailor", LastName = "One", Email = "tailor@test.com", UserName = "tailor" };
            var customer = new Customer { FullName = "Amara Okafor", Phone = "555-0001" };
            var workSpace = new WorkSpace { Name = "WS" };
            context.Users.Add(tailor);
            context.Customers.Add(customer);
            context.WorkSpaces.Add(workSpace);
            await context.SaveChangesAsync();
            context.WorkSpaceMemberships.Add(new WorkSpaceMembership { UserId = tailor.Id, WorkSpaceId = workSpace.Id, Role = Role.Tailor });
            await context.SaveChangesAsync();
            var service = CreateService(context);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                service.AssignCustomerToWorkSpaceAsync(customer.Id, workSpace.Id, tailor.Id, CancellationToken.None));
        }

        [Fact]
        public async Task AssignCustomerToWorkSpaceAsync_CustomerNotFound_Throws()
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

            await Assert.ThrowsAsync<NotFoundException>(() =>
                service.AssignCustomerToWorkSpaceAsync(999, workSpace.Id, owner.Id, CancellationToken.None));
        }

        [Fact]
        public async Task DeactivateThenReactivateCustomerAsync_AssignedCustomer_OwnerCanToggle()
        {
            using var context = TestHelpers.CreateContext();
            var owner = new User { FirstName = "Owner", LastName = "One", Email = "owner@test.com", UserName = "owner" };
            var customer = new Customer { FullName = "Amara Okafor", Phone = "555-0001" };
            var workSpace = new WorkSpace { Name = "WS" };
            context.Users.Add(owner);
            context.Customers.Add(customer);
            context.WorkSpaces.Add(workSpace);
            await context.SaveChangesAsync();
            context.WorkSpaceMemberships.Add(new WorkSpaceMembership { UserId = owner.Id, WorkSpaceId = workSpace.Id, Role = Role.Owner });
            await context.SaveChangesAsync();
            var service = CreateService(context);
            await service.AssignCustomerToWorkSpaceAsync(customer.Id, workSpace.Id, owner.Id, CancellationToken.None);

            var deactivated = await service.DeactivateCustomerAsync(customer.Id, owner.Id, CancellationToken.None);
            Assert.False(deactivated.IsActive);

            var reactivated = await service.ReactivateCustomerAsync(customer.Id, owner.Id, CancellationToken.None);
            Assert.True(reactivated.IsActive);
        }

        [Fact]
        public async Task DeactivateCustomerAsync_AssignedCustomer_TailorCannot()
        {
            using var context = TestHelpers.CreateContext();
            var owner = new User { FirstName = "Owner", LastName = "One", Email = "owner@test.com", UserName = "owner" };
            var tailor = new User { FirstName = "Tailor", LastName = "One", Email = "tailor@test.com", UserName = "tailor" };
            var customer = new Customer { FullName = "Amara Okafor", Phone = "555-0001" };
            var workSpace = new WorkSpace { Name = "WS" };
            context.Users.AddRange(owner, tailor);
            context.Customers.Add(customer);
            context.WorkSpaces.Add(workSpace);
            await context.SaveChangesAsync();
            context.WorkSpaceMemberships.Add(new WorkSpaceMembership { UserId = owner.Id, WorkSpaceId = workSpace.Id, Role = Role.Owner });
            context.WorkSpaceMemberships.Add(new WorkSpaceMembership { UserId = tailor.Id, WorkSpaceId = workSpace.Id, Role = Role.Tailor });
            await context.SaveChangesAsync();
            var service = CreateService(context);
            await service.AssignCustomerToWorkSpaceAsync(customer.Id, workSpace.Id, owner.Id, CancellationToken.None);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                service.DeactivateCustomerAsync(customer.Id, tailor.Id, CancellationToken.None));
        }

        [Fact]
        public async Task DeactivateCustomerAsync_UnassignedCustomer_OnlyCreatorCan()
        {
            using var context = TestHelpers.CreateContext();
            var creator = new User { FirstName = "Owner", LastName = "One", Email = "owner@test.com", UserName = "owner" };
            var someoneElse = new User { FirstName = "Other", LastName = "One", Email = "other@test.com", UserName = "other" };
            context.Users.AddRange(creator, someoneElse);
            await context.SaveChangesAsync();
            var service = CreateService(context);
            var customer = await service.CreateCustomerAsync(
                new CustomerRequestDTO { FullName = "Amara Okafor", Phone = "555-0001" }, creator.Id, CancellationToken.None);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                service.DeactivateCustomerAsync(customer.Id, someoneElse.Id, CancellationToken.None));

            var deactivated = await service.DeactivateCustomerAsync(customer.Id, creator.Id, CancellationToken.None);
            Assert.False(deactivated.IsActive);
        }
    }
}
