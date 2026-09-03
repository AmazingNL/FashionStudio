using FashionStudio.Api.Data;
using FashionStudio.Api.DTOs;
using FashionStudio.Api.Exceptions;
using FashionStudio.Api.Models;
using FashionStudio.Api.Services;
using Xunit;

namespace FashionStudio.Api.Tests
{
    public class PaymentServiceTests
    {
        private static async Task<(AppDbContext Context, WorkSpace WorkSpace, Order Order, int OwnerId)> SeedOrderAsync(
            AppDbContext context, decimal quotedPrice = 100m, decimal discount = 0m)
        {
            var owner = new User { FirstName = "Owner", LastName = "One", Email = "owner@test.com", UserName = "owner" };
            var customer = new Customer { FullName = "Test Customer", Phone = "555" };
            var workSpace = new WorkSpace { Name = "WS" };

            context.Users.Add(owner);
            context.Customers.Add(customer);
            context.WorkSpaces.Add(workSpace);
            await context.SaveChangesAsync();

            context.WorkSpaceMemberships.Add(new WorkSpaceMembership { UserId = owner.Id, WorkSpaceId = workSpace.Id, Role = Role.Owner });

            var order = new Order
            {
                CustomerId = customer.Id,
                WorkSpaceId = workSpace.Id,
                CreatedByUserId = owner.Id,
                Title = "Order",
                QuotedPrice = quotedPrice,
                Discount = discount
            };
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            return (context, workSpace, order, owner.Id);
        }

        private static PaymentService CreateService(AppDbContext context)
        {
            var mapper = TestHelpers.CreateMapper();
            var workSpaceService = new WorkSpaceService(context, mapper, new NotImplementedUserService());
            return new PaymentService(context, mapper, workSpaceService);
        }

        [Fact]
        public async Task CreatePaymentAsync_WithinBalance_Succeeds()
        {
            using var context = TestHelpers.CreateContext();
            var (_, _, order, ownerId) = await SeedOrderAsync(context, quotedPrice: 100m);
            var service = CreateService(context);

            var result = await service.CreatePaymentAsync(
                new PaymentRequestDTO { OrderId = order.Id, Amount = 60m }, ownerId, CancellationToken.None);

            Assert.Equal(60m, result.Amount);
        }

        [Fact]
        public async Task CreatePaymentAsync_ExceedingBalance_Throws()
        {
            using var context = TestHelpers.CreateContext();
            var (_, _, order, ownerId) = await SeedOrderAsync(context, quotedPrice: 100m);
            var service = CreateService(context);

            await service.CreatePaymentAsync(
                new PaymentRequestDTO { OrderId = order.Id, Amount = 60m }, ownerId, CancellationToken.None);

            // A second payment that would push the total past the 100 owed must be rejected.
            await Assert.ThrowsAsync<ConflictException>(() =>
                service.CreatePaymentAsync(
                    new PaymentRequestDTO { OrderId = order.Id, Amount = 50m }, ownerId, CancellationToken.None));
        }

        [Fact]
        public async Task CreatePaymentAsync_AccountsForDiscount()
        {
            using var context = TestHelpers.CreateContext();
            var (_, _, order, ownerId) = await SeedOrderAsync(context, quotedPrice: 100m, discount: 20m);
            var service = CreateService(context);

            // Balance due is 100 - 20 = 80, so 80 should be accepted but nothing more.
            await service.CreatePaymentAsync(
                new PaymentRequestDTO { OrderId = order.Id, Amount = 80m }, ownerId, CancellationToken.None);

            await Assert.ThrowsAsync<ConflictException>(() =>
                service.CreatePaymentAsync(
                    new PaymentRequestDTO { OrderId = order.Id, Amount = 1m }, ownerId, CancellationToken.None));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task CreatePaymentAsync_NonPositiveAmount_Throws(decimal amount)
        {
            using var context = TestHelpers.CreateContext();
            var (_, _, order, ownerId) = await SeedOrderAsync(context);
            var service = CreateService(context);

            await Assert.ThrowsAsync<ConflictException>(() =>
                service.CreatePaymentAsync(
                    new PaymentRequestDTO { OrderId = order.Id, Amount = amount }, ownerId, CancellationToken.None));
        }

        [Fact]
        public async Task CreatePaymentAsync_UserNotOwnerOrAssistant_Throws()
        {
            using var context = TestHelpers.CreateContext();
            var (_, workSpace, order, _) = await SeedOrderAsync(context);

            var tailor = new User { FirstName = "Tailor", LastName = "One", Email = "tailor@test.com", UserName = "tailor" };
            context.Users.Add(tailor);
            await context.SaveChangesAsync();
            context.WorkSpaceMemberships.Add(new WorkSpaceMembership { UserId = tailor.Id, WorkSpaceId = workSpace.Id, Role = Role.Tailor });
            await context.SaveChangesAsync();

            var service = CreateService(context);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                service.CreatePaymentAsync(
                    new PaymentRequestDTO { OrderId = order.Id, Amount = 10m }, tailor.Id, CancellationToken.None));
        }

        [Fact]
        public async Task CreatePaymentAsync_OrderNotFound_Throws()
        {
            using var context = TestHelpers.CreateContext();
            var (_, _, _, ownerId) = await SeedOrderAsync(context);
            var service = CreateService(context);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                service.CreatePaymentAsync(
                    new PaymentRequestDTO { OrderId = 999, Amount = 10m }, ownerId, CancellationToken.None));
        }
    }
}
