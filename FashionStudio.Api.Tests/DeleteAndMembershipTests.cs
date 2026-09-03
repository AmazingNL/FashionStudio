using FashionStudio.Api.DTOs;
using FashionStudio.Api.Exceptions;
using FashionStudio.Api.Models;
using FashionStudio.Api.Services;
using Xunit;

namespace FashionStudio.Api.Tests
{
    public class DeleteAndMembershipTests
    {
        private static (OrderService OrderService, PaymentService PaymentService) CreateOrderAndPaymentServices(Data.AppDbContext context)
        {
            var mapper = TestHelpers.CreateMapper();
            var workSpaceService = new WorkSpaceService(context, mapper, new NotImplementedUserService());
            var orderImageService = new NoOpOrderImageService();
            var orderService = new OrderService(context, mapper, workSpaceService, orderImageService);
            var paymentService = new PaymentService(context, mapper, workSpaceService);
            return (orderService, paymentService);
        }

        private static async Task<(WorkSpace WorkSpace, Order Order, int OwnerId)> SeedOrderAsync(Data.AppDbContext context)
        {
            var owner = new User { FirstName = "Owner", LastName = "One", Email = "owner@test.com", UserName = "owner" };
            var customer = new Customer { FullName = "Customer", Phone = "555" };
            var workSpace = new WorkSpace { Name = "WS" };
            context.Users.Add(owner);
            context.Customers.Add(customer);
            context.WorkSpaces.Add(workSpace);
            await context.SaveChangesAsync();
            context.WorkSpaceMemberships.Add(new WorkSpaceMembership { UserId = owner.Id, WorkSpaceId = workSpace.Id, Role = Role.Owner });

            var order = new Order { CustomerId = customer.Id, WorkSpaceId = workSpace.Id, CreatedByUserId = owner.Id, Title = "Order", QuotedPrice = 100m };
            context.Orders.Add(order);
            await context.SaveChangesAsync();

            return (workSpace, order, owner.Id);
        }

        [Fact]
        public async Task DeleteOrderAsync_NoPayments_Succeeds()
        {
            using var context = TestHelpers.CreateContext();
            var (_, order, ownerId) = await SeedOrderAsync(context);
            var (orderService, _) = CreateOrderAndPaymentServices(context);

            await orderService.DeleteOrderAsync(order.Id, ownerId, CancellationToken.None);

            Assert.Empty(context.Orders);
        }

        [Fact]
        public async Task DeleteOrderAsync_WithRecordedPayments_Throws()
        {
            using var context = TestHelpers.CreateContext();
            var (_, order, ownerId) = await SeedOrderAsync(context);
            var (orderService, paymentService) = CreateOrderAndPaymentServices(context);
            await paymentService.CreatePaymentAsync(
                new PaymentRequestDTO { OrderId = order.Id, Amount = 10m }, ownerId, CancellationToken.None);

            // Deleting the order would cascade-delete the payment record too — that's exactly
            // the "silently erase the ledger" scenario the immutability decision was meant to
            // prevent, so it must be blocked instead.
            await Assert.ThrowsAsync<ConflictException>(() =>
                orderService.DeleteOrderAsync(order.Id, ownerId, CancellationToken.None));
            Assert.Single(context.Orders);
        }

        [Fact]
        public async Task UpdateMemberRoleAsync_DemotingSoleOwner_Throws()
        {
            using var context = TestHelpers.CreateContext();
            var owner = new User { FirstName = "Owner", LastName = "One", Email = "owner@test.com", UserName = "owner" };
            var workSpace = new WorkSpace { Name = "WS" };
            context.Users.Add(owner);
            context.WorkSpaces.Add(workSpace);
            await context.SaveChangesAsync();
            context.WorkSpaceMemberships.Add(new WorkSpaceMembership { UserId = owner.Id, WorkSpaceId = workSpace.Id, Role = Role.Owner });
            await context.SaveChangesAsync();
            var service = new WorkSpaceService(context, TestHelpers.CreateMapper(), new NotImplementedUserService());

            await Assert.ThrowsAsync<ConflictException>(() =>
                service.UpdateMemberRoleAsync(workSpace.Id, owner.Id, Role.Assistant, owner.Id, CancellationToken.None));
        }

        [Fact]
        public async Task UpdateMemberRoleAsync_DemotingOneOfTwoOwners_Succeeds()
        {
            using var context = TestHelpers.CreateContext();
            var owner1 = new User { FirstName = "Owner", LastName = "One", Email = "owner1@test.com", UserName = "owner1" };
            var owner2 = new User { FirstName = "Owner", LastName = "Two", Email = "owner2@test.com", UserName = "owner2" };
            var workSpace = new WorkSpace { Name = "WS" };
            context.Users.AddRange(owner1, owner2);
            context.WorkSpaces.Add(workSpace);
            await context.SaveChangesAsync();
            context.WorkSpaceMemberships.Add(new WorkSpaceMembership { UserId = owner1.Id, WorkSpaceId = workSpace.Id, Role = Role.Owner });
            context.WorkSpaceMemberships.Add(new WorkSpaceMembership { UserId = owner2.Id, WorkSpaceId = workSpace.Id, Role = Role.Owner });
            await context.SaveChangesAsync();
            var service = new WorkSpaceService(context, TestHelpers.CreateMapper(), new NotImplementedUserService());

            var result = await service.UpdateMemberRoleAsync(workSpace.Id, owner2.Id, Role.Assistant, owner1.Id, CancellationToken.None);

            Assert.Equal(Role.Assistant, result.Role);
        }

        [Fact]
        public async Task RemoveMemberAsync_RemovingSoleOwner_Throws()
        {
            using var context = TestHelpers.CreateContext();
            var owner = new User { FirstName = "Owner", LastName = "One", Email = "owner@test.com", UserName = "owner" };
            var workSpace = new WorkSpace { Name = "WS" };
            context.Users.Add(owner);
            context.WorkSpaces.Add(workSpace);
            await context.SaveChangesAsync();
            context.WorkSpaceMemberships.Add(new WorkSpaceMembership { UserId = owner.Id, WorkSpaceId = workSpace.Id, Role = Role.Owner });
            await context.SaveChangesAsync();
            var service = new WorkSpaceService(context, TestHelpers.CreateMapper(), new NotImplementedUserService());

            await Assert.ThrowsAsync<ConflictException>(() =>
                service.RemoveMemberAsync(workSpace.Id, owner.Id, owner.Id, CancellationToken.None));
        }

        [Fact]
        public async Task RemoveMemberAsync_RemovingTailor_DoesNotDeleteUnderlyingUser()
        {
            using var context = TestHelpers.CreateContext();
            var owner = new User { FirstName = "Owner", LastName = "One", Email = "owner@test.com", UserName = "owner" };
            var tailor = new User { FirstName = "Tailor", LastName = "One", Email = "tailor@test.com", UserName = "tailor" };
            var workSpace = new WorkSpace { Name = "WS" };
            context.Users.AddRange(owner, tailor);
            context.WorkSpaces.Add(workSpace);
            await context.SaveChangesAsync();
            context.WorkSpaceMemberships.Add(new WorkSpaceMembership { UserId = owner.Id, WorkSpaceId = workSpace.Id, Role = Role.Owner });
            context.WorkSpaceMemberships.Add(new WorkSpaceMembership { UserId = tailor.Id, WorkSpaceId = workSpace.Id, Role = Role.Tailor });
            await context.SaveChangesAsync();
            var service = new WorkSpaceService(context, TestHelpers.CreateMapper(), new NotImplementedUserService());

            await service.RemoveMemberAsync(workSpace.Id, tailor.Id, owner.Id, CancellationToken.None);

            Assert.Single(context.WorkSpaceMemberships);
            // The membership is gone, but the User account itself must still exist.
            Assert.Equal(2, context.Users.Count());
        }

        [Fact]
        public async Task RemoveMemberAsync_CallerNotOwner_Throws()
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
            var service = new WorkSpaceService(context, TestHelpers.CreateMapper(), new NotImplementedUserService());

            // Even an Assistant — who can create/manage orders — can't manage membership; that's Owner-only.
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                service.RemoveMemberAsync(workSpace.Id, owner.Id, assistant.Id, CancellationToken.None));
        }
    }

    // OrderService.DeleteOrderAsync only needs DeleteImagesForOrderAsync to not throw —
    // the file-cleanup behavior itself is OrderImageService's own concern and already covered
    // by not needing a real IWebHostEnvironment/StorageSettings pair in these tests.
    public class NoOpOrderImageService : Interfaces.IOrderImageService
    {
        public Task<OrderImageResponseDTO> UploadImageAsync(OrderImageUploadDTO request, int userId, CancellationToken cancellation) => throw new NotImplementedException();
        public Task<OrderImageResponseDTO> GetImageByIdAsync(int imageId) => throw new NotImplementedException();
        public Task<(Stream Stream, string ContentType, string FileName)> GetImageFileAsync(int imageId, CancellationToken cancellation) => throw new NotImplementedException();
        public Task<PageResultDTO<OrderImageResponseDTO>> GetAllImagesAsync(QueryParam queryParam, CancellationToken cancellation) => throw new NotImplementedException();
        public Task DeleteImageAsync(int imageId, int actingUserId, CancellationToken cancellation) => throw new NotImplementedException();
        public Task DeleteImagesForOrderAsync(int orderId, CancellationToken cancellation) => Task.CompletedTask;
    }
}
