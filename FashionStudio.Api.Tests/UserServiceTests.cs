using FashionStudio.Api.DTOs;
using FashionStudio.Api.Exceptions;
using FashionStudio.Api.Models;
using FashionStudio.Api.Services;
using Xunit;

namespace FashionStudio.Api.Tests
{
    public class UserServiceTests
    {
        private static UserService CreateService(Data.AppDbContext context) =>
            new(context, TestHelpers.CreateMapper());

        private static RegisterRequestDTO ValidRegisterRequest(string email = "amara@test.com", string userName = "amara") => new()
        {
            FirstName = "Amara",
            LastName = "Okafor",
            UserName = userName,
            Email = email,
            Password = "Str0ngPass"
        };

        [Fact]
        public async Task RegisterUserAsync_ValidRequest_HashesPasswordAndReturnsUser()
        {
            using var context = TestHelpers.CreateContext();
            var service = CreateService(context);

            var result = await service.RegisterUserAsync(ValidRegisterRequest());

            var stored = Assert.Single(context.Users);
            Assert.Equal("amara@test.com", result.Email);
            // The raw password must never be persisted as-is.
            Assert.NotEqual("Str0ngPass", stored.Password);
        }

        [Fact]
        public async Task RegisterUserAsync_DuplicateEmail_Throws()
        {
            using var context = TestHelpers.CreateContext();
            var service = CreateService(context);
            await service.RegisterUserAsync(ValidRegisterRequest());

            await Assert.ThrowsAsync<ConflictException>(() =>
                service.RegisterUserAsync(ValidRegisterRequest(userName: "amara2")));
        }

        [Fact]
        public async Task RegisterUserAsync_WeakPassword_Throws()
        {
            // Documents current behavior: UserService.ValidatePass rejects passwords that are
            // too short or missing an upper/lower/digit, but HashPassword's catch block wraps
            // that rejection in InvalidOperationException — which GlobalExceptionHandler maps to
            // a generic 500, not a client-friendly 400 with the actual validation reason.
            using var context = TestHelpers.CreateContext();
            var service = CreateService(context);
            var weak = ValidRegisterRequest();
            weak.Password = "short";

            await Assert.ThrowsAsync<InvalidOperationException>(() => service.RegisterUserAsync(weak));
        }

        [Fact]
        public async Task RegisterUserAsync_WithPendingInvitation_AutoJoinsWorkSpace()
        {
            using var context = TestHelpers.CreateContext();
            var owner = new User { FirstName = "Owner", LastName = "One", Email = "owner@test.com", UserName = "owner" };
            var workSpace = new WorkSpace { Name = "WS" };
            context.Users.Add(owner);
            context.WorkSpaces.Add(workSpace);
            await context.SaveChangesAsync();

            context.WorkSpaceInvitations.Add(new WorkSpaceInvitation
            {
                WorkSpaceId = workSpace.Id,
                OwnerId = owner.Id,
                Email = "amara@test.com",
                Role = Role.Tailor,
                ExpiresAt = DateTime.UtcNow.AddDays(1)
            });
            await context.SaveChangesAsync();

            var service = CreateService(context);
            var result = await service.RegisterUserAsync(ValidRegisterRequest());

            var membership = Assert.Single(context.WorkSpaceMemberships);
            Assert.Equal(workSpace.Id, membership.WorkSpaceId);
            Assert.Equal(Role.Tailor, membership.Role);
            Assert.Empty(context.WorkSpaceInvitations);
        }

        [Fact]
        public async Task RegisterThenVerifyPasswordAsync_CorrectCredentials_ReturnsUser()
        {
            using var context = TestHelpers.CreateContext();
            var service = CreateService(context);
            await service.RegisterUserAsync(ValidRegisterRequest());

            var user = await service.VerifyPasswordAsync(new LoginRequestDTO { UserName = "amara", Password = "Str0ngPass" });

            Assert.Equal("amara", user.UserName);
        }

        [Fact]
        public async Task VerifyPasswordAsync_WrongPassword_Throws()
        {
            using var context = TestHelpers.CreateContext();
            var service = CreateService(context);
            await service.RegisterUserAsync(ValidRegisterRequest());

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                service.VerifyPasswordAsync(new LoginRequestDTO { UserName = "amara", Password = "WrongPass1" }));
        }

        [Fact]
        public async Task VerifyPasswordAsync_UnknownUserName_Throws()
        {
            using var context = TestHelpers.CreateContext();
            var service = CreateService(context);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
                service.VerifyPasswordAsync(new LoginRequestDTO { UserName = "ghost", Password = "Str0ngPass" }));
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsPagedResults()
        {
            using var context = TestHelpers.CreateContext();
            var service = CreateService(context);
            await service.RegisterUserAsync(ValidRegisterRequest());
            await service.RegisterUserAsync(ValidRegisterRequest(email: "second@test.com", userName: "second"));

            var page = await service.GetAllUsersAsync(new QueryParam { PageNumber = 1, PageSize = 10 }, CancellationToken.None);

            Assert.Equal(2, page.TotalCount);
        }
    }
}
