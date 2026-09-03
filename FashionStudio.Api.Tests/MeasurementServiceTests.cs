using FashionStudio.Api.DTOs;
using FashionStudio.Api.Exceptions;
using FashionStudio.Api.Models;
using FashionStudio.Api.Services;
using Xunit;

namespace FashionStudio.Api.Tests
{
    public class MeasurementServiceTests
    {
        private static MeasurementService CreateService(Data.AppDbContext context) =>
            new(context, TestHelpers.CreateMapper());

        [Fact]
        public async Task CreateMeasurementAsync_ValidRequest_PersistsFields()
        {
            using var context = TestHelpers.CreateContext();
            var user = new User { FirstName = "Tailor", LastName = "One", Email = "tailor@test.com", UserName = "tailor" };
            var customer = new Customer { FullName = "Amara Okafor", Phone = "555" };
            context.Users.Add(user);
            context.Customers.Add(customer);
            await context.SaveChangesAsync();
            var service = CreateService(context);

            var result = await service.CreateMeasurementAsync(
                new MeasurementRequestDTO
                {
                    CustomerId = customer.Id,
                    Label = "Wedding fitting",
                    Unit = Unit.Cm,
                    DateTaken = DateTime.UtcNow,
                    Fields = new MeasurementFieldDTO { Bust = 90m, Waist = 70m, CustomMeasurements = { ["Wrist"] = 15m } }
                },
                user.Id, CancellationToken.None);

            Assert.Equal("Wedding fitting", result.Label);
            var field = Assert.Single(result.Fields);
            Assert.Equal(90m, field.Bust);
            Assert.Equal(15m, field.CustomMeasurements["Wrist"]);
        }

        [Fact]
        public async Task CreateMeasurementAsync_CustomerNotFound_Throws()
        {
            using var context = TestHelpers.CreateContext();
            var user = new User { FirstName = "Tailor", LastName = "One", Email = "tailor@test.com", UserName = "tailor" };
            context.Users.Add(user);
            await context.SaveChangesAsync();
            var service = CreateService(context);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                service.CreateMeasurementAsync(
                    new MeasurementRequestDTO { CustomerId = 999 }, user.Id, CancellationToken.None));
        }

        [Fact]
        public async Task CreateMeasurementAsync_ActingUserNotFound_Throws()
        {
            using var context = TestHelpers.CreateContext();
            var customer = new Customer { FullName = "Amara Okafor", Phone = "555" };
            context.Customers.Add(customer);
            await context.SaveChangesAsync();
            var service = CreateService(context);

            await Assert.ThrowsAsync<NotFoundException>(() =>
                service.CreateMeasurementAsync(
                    new MeasurementRequestDTO { CustomerId = customer.Id }, 999, CancellationToken.None));
        }
    }
}
