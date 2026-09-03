using FashionStudio.Api.Data;
using FashionStudio.Api.DTOs;
using FashionStudio.Api.Exceptions;
using FashionStudio.Api.Models;
using FashionStudio.Api.Services;
using Xunit;

namespace FashionStudio.Api.Tests
{
    public class OrderServiceTests
    {
        private static OrderService CreateService(AppDbContext context)
        {
            var mapper = TestHelpers.CreateMapper();
            var workSpaceService = new WorkSpaceService(context, mapper, new NotImplementedUserService());
            return new OrderService(context, mapper, workSpaceService, new NoOpOrderImageService());
        }

        private static async Task<(WorkSpace WorkSpace, Customer Customer, int OwnerId)> SeedWorkSpaceWithCustomerAsync(AppDbContext context)
        {
            var owner = new User { FirstName = "Owner", LastName = "One", Email = "owner@test.com", UserName = "owner" };
            var workSpace = new WorkSpace { Name = "WS" };
            var customer = new Customer { FullName = "Customer", Phone = "555", WorkSpaceId = null };

            context.Users.Add(owner);
            context.WorkSpaces.Add(workSpace);
            context.Customers.Add(customer);
            await context.SaveChangesAsync();

            customer.WorkSpaceId = workSpace.Id;
            context.WorkSpaceMemberships.Add(new WorkSpaceMembership { UserId = owner.Id, WorkSpaceId = workSpace.Id, Role = Role.Owner });
            await context.SaveChangesAsync();

            return (workSpace, customer, owner.Id);
        }

        [Fact]
        public async Task CreateOrderAsync_ValidRequest_CreatesUnassignedOrder()
        {
            using var context = TestHelpers.CreateContext();
            var (workSpace, customer, ownerId) = await SeedWorkSpaceWithCustomerAsync(context);
            var service = CreateService(context);

            var result = await service.CreateOrderAsync(
                new OrderRequestDTO { WorkSpaceId = workSpace.Id, CustomerId = customer.Id, Title = "Suit" },
                ownerId, CancellationToken.None);

            Assert.Null(result.AssignedToUserId);
        }

        [Fact]
        public async Task CreateOrderAsync_CustomerBelongsToDifferentWorkSpace_Throws()
        {
            using var context = TestHelpers.CreateContext();
            var (workSpace, customer, ownerId) = await SeedWorkSpaceWithCustomerAsync(context);

            var otherWorkSpace = new WorkSpace { Name = "Other" };
            context.WorkSpaces.Add(otherWorkSpace);
            await context.SaveChangesAsync();
            context.WorkSpaceMemberships.Add(new WorkSpaceMembership { UserId = ownerId, WorkSpaceId = otherWorkSpace.Id, Role = Role.Owner });
            await context.SaveChangesAsync();

            var service = CreateService(context);

            // customer belongs to `workSpace`, but the request targets `otherWorkSpace`.
            await Assert.ThrowsAsync<ConflictException>(() =>
                service.CreateOrderAsync(
                    new OrderRequestDTO { WorkSpaceId = otherWorkSpace.Id, CustomerId = customer.Id, Title = "Suit" },
                    ownerId, CancellationToken.None));
        }

        [Fact]
        public async Task CreateOrderAsync_AssignedUserNotWorkSpaceMember_Throws()
        {
            using var context = TestHelpers.CreateContext();
            var (workSpace, customer, ownerId) = await SeedWorkSpaceWithCustomerAsync(context);

            var outsider = new User { FirstName = "Out", LastName = "Sider", Email = "outsider@test.com", UserName = "outsider" };
            context.Users.Add(outsider);
            await context.SaveChangesAsync();

            var service = CreateService(context);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                service.CreateOrderAsync(
                    new OrderRequestDTO
                    {
                        WorkSpaceId = workSpace.Id,
                        CustomerId = customer.Id,
                        Title = "Suit",
                        AssignedToUserId = outsider.Id
                    },
                    ownerId, CancellationToken.None));
        }

        [Fact]
        public async Task CreateOrderAsync_CallerNotOwnerOrAssistant_Throws()
        {
            using var context = TestHelpers.CreateContext();
            var (workSpace, customer, _) = await SeedWorkSpaceWithCustomerAsync(context);

            var tailor = new User { FirstName = "Tailor", LastName = "One", Email = "tailor@test.com", UserName = "tailor" };
            context.Users.Add(tailor);
            await context.SaveChangesAsync();
            context.WorkSpaceMemberships.Add(new WorkSpaceMembership { UserId = tailor.Id, WorkSpaceId = workSpace.Id, Role = Role.Tailor });
            await context.SaveChangesAsync();

            var service = CreateService(context);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                service.CreateOrderAsync(
                    new OrderRequestDTO { WorkSpaceId = workSpace.Id, CustomerId = customer.Id, Title = "Suit" },
                    tailor.Id, CancellationToken.None));
        }
    }
}
