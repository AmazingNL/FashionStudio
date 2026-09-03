using System.Net;
using System.Net.Http.Json;
using FashionStudio.Api.DTOs;
using Xunit;

namespace FashionStudio.Api.Tests.Integration
{
    public class AuthIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public AuthIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        private static RegisterRequestDTO NewRegisterRequest(string suffix) => new()
        {
            FirstName = "Amara",
            LastName = "Okafor",
            UserName = $"amara_{suffix}",
            Email = $"amara_{suffix}@test.com",
            Password = "Str0ngPass"
        };

        [Fact]
        public async Task Register_ValidRequest_ReturnsCreatedUser()
        {
            var request = NewRegisterRequest(nameof(Register_ValidRequest_ReturnsCreatedUser));

            var response = await _client.PostAsJsonAsync("/api/auth/register", request);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var user = await response.ReadAsAsync<UserResponseDTO>();
            Assert.Equal(request.Email, user!.Email);
        }

        [Fact]
        public async Task Register_DuplicateEmail_ReturnsConflict()
        {
            var request = NewRegisterRequest(nameof(Register_DuplicateEmail_ReturnsConflict));
            await _client.PostAsJsonAsync("/api/auth/register", request);

            var second = new RegisterRequestDTO
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                UserName = request.UserName + "_2",
                Email = request.Email, // same email
                Password = request.Password
            };
            var response = await _client.PostAsJsonAsync("/api/auth/register", second);

            // Exercises the real pipeline: ConflictException thrown by UserService must reach
            // GlobalExceptionHandler and come back as an actual HTTP 409, not a 500.
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsBearerToken()
        {
            var request = NewRegisterRequest(nameof(Login_ValidCredentials_ReturnsBearerToken));
            await _client.PostAsJsonAsync("/api/auth/register", request);

            var response = await _client.PostAsJsonAsync("/api/auth/login",
                new LoginRequestDTO { UserName = request.UserName, Password = request.Password });

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.ReadAsAsync<Dictionary<string, string>>();
            Assert.False(string.IsNullOrWhiteSpace(body!["token"]));
        }

        [Fact]
        public async Task Login_WrongPassword_ReturnsUnauthorized()
        {
            var request = NewRegisterRequest(nameof(Login_WrongPassword_ReturnsUnauthorized));
            await _client.PostAsJsonAsync("/api/auth/register", request);

            var response = await _client.PostAsJsonAsync("/api/auth/login",
                new LoginRequestDTO { UserName = request.UserName, Password = "WrongPass1" });

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task ProtectedEndpoint_WithoutToken_ReturnsUnauthorized()
        {
            var response = await _client.GetAsync("/api/customer/list");

            // Confirms [Authorize] + the JWT bearer scheme are actually wired up end to end.
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task ProtectedEndpoint_WithValidToken_Succeeds()
        {
            var request = NewRegisterRequest(nameof(ProtectedEndpoint_WithValidToken_Succeeds));
            await _client.PostAsJsonAsync("/api/auth/register", request);
            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login",
                new LoginRequestDTO { UserName = request.UserName, Password = request.Password });
            var token = (await loginResponse.ReadAsAsync<Dictionary<string, string>>())!["token"];

            using var authedRequest = new HttpRequestMessage(HttpMethod.Get, "/api/customer/list");
            authedRequest.Headers.Add("Authorization", $"Bearer {token}");
            var response = await _client.SendAsync(authedRequest);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
