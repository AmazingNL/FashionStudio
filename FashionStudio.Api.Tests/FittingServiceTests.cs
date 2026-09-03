using FashionStudio.Api.Data;
using FashionStudio.Api.DTOs;
using FashionStudio.Api.Exceptions;
using FashionStudio.Api.Models;
using FashionStudio.Api.Services;
using Xunit;

namespace FashionStudio.Api.Tests
{
    public class FittingServiceTests
    {
        private static FittingService CreateService(AppDbContext context)
        {
            var mapper = TestHelpers.CreateMapper();
            var workSpaceService = new WorkSpaceService(context, mapper, new NotImplementedUserService());
            return new FittingService(context, mapper, workSpaceService);
        }

        private static async Task<(Order Order, int OwnerId)> SeedOrderAsync(AppDbContext context)
        {
            var owner = new User { FirstName = "Owner", LastName = "One", Email = "owner@test.com", UserName = "owner" };
            var customer = new Customer { FullName = "Customer", Phone = "555" };
            var workSpace = new WorkSpace { Name = "WS" };

            context.Users.Add(owner);
            context.Customers.Add(customer);
            context.WorkSpaces.Add(workSpace);
            await context.SaveChangesAsync();
            context.WorkSpaceMemberships.Add(new WorkSpaceMembership { UserId = owner.Id, WorkSpaceId = workSpace.Id, Role = Role.Owner });

            var order = new Order { CustomerId = customer.Id, WorkSpaceId = workSpace.Id, CreatedByUserId = owner.Id, Title = "Order" };
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            return (order, owner.Id);
        }

        [Fact]
        public async Task CreateFittingAsync_ValidRequest_DerivesWorkSpaceAndCustomerFromOrder()
        {
            using var context = TestHelpers.CreateContext();
            var (order, ownerId) = await SeedOrderAsync(context);
            var service = CreateService(context);

            var result = await service.CreateFittingAsync(
                new FittingRequestDTO { OrderId = order.Id, FittingDate = DateTime.UtcNow },
                ownerId, CancellationToken.None);

            Assert.Equal(order.WorkSpaceId, result.WorkSpaceId);
            Assert.Equal(order.CustomerId, result.CustomerId);
            Assert.Equal(FittingOutcome.Pending, result.Outcome);
        }

        [Fact]
        public async Task CreateFittingAsync_OrderNotFound_Throws()
        {
            using var context = TestHelpers.CreateContext();
            var (_, ownerId) = await SeedOrderAsync(context);
            var service = CreateService(context);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                service.CreateFittingAsync(
                    new FittingRequestDTO { OrderId = 999, FittingDate = DateTime.UtcNow },
                    ownerId, CancellationToken.None));
        }

        [Fact]
        public async Task UpdateFittingAsync_SetsOutcomeAndLeavesOtherFieldsUntouched()
        {
            using var context = TestHelpers.CreateContext();
            var (order, ownerId) = await SeedOrderAsync(context);
            var service = CreateService(context);

            var created = await service.CreateFittingAsync(
                new FittingRequestDTO { OrderId = order.Id, FittingDate = DateTime.UtcNow, Notes = "Initial" },
                ownerId, CancellationToken.None);

            var updated = await service.UpdateFittingAsync(
                created.Id, new FittingUpdateDTO { Outcome = FittingOutcome.Approved }, ownerId, CancellationToken.None);

            Assert.Equal(FittingOutcome.Approved, updated.Outcome);
            Assert.Equal("Initial", updated.Notes);
        }
    }
}
