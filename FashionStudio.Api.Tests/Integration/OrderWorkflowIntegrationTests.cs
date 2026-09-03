using System.Net;
using System.Net.Http.Json;
using FashionStudio.Api.DTOs;
using FashionStudio.Api.Models;
using Xunit;

namespace FashionStudio.Api.Tests.Integration
{
    public class OrderWorkflowIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public OrderWorkflowIntegrationTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        private async Task<string> RegisterAndLoginAsync(string suffix)
        {
            var register = new RegisterRequestDTO
            {
                FirstName = "Test",
                LastName = "User",
                UserName = $"user_{suffix}",
                Email = $"user_{suffix}@test.com",
                Password = "Str0ngPass"
            };
            var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", register);
            Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login",
                new LoginRequestDTO { UserName = register.UserName, Password = register.Password });
            Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

            var body = await loginResponse.ReadAsAsync<Dictionary<string, string>>();
            return body!["token"];
        }

        private static HttpRequestMessage Authed(HttpMethod method, string url, string token, object? body = null)
        {
            var request = new HttpRequestMessage(method, url);
            request.Headers.Add("Authorization", $"Bearer {token}");
            if (body != null) request.Content = JsonContent.Create(body);
            return request;
        }

        [Fact]
        public async Task FullOrderCreationFlow_AsOwner_Succeeds()
        {
            var suffix = nameof(FullOrderCreationFlow_AsOwner_Succeeds);
            var token = await RegisterAndLoginAsync(suffix);

            var workSpaceResponse = await _client.SendAsync(Authed(HttpMethod.Post, "/api/workspace/create", token,
                new WorkSpaceRequestDTO { Name = $"Grace Couture {suffix}" }));
            Assert.Equal(HttpStatusCode.Created, workSpaceResponse.StatusCode);
            var workSpace = await workSpaceResponse.ReadAsAsync<WorkSpaceResponseDTO>();

            var customerResponse = await _client.SendAsync(Authed(HttpMethod.Post, "/api/customer/create", token,
                new CustomerRequestDTO { FullName = "Amara Okafor", Phone = $"555-{suffix}" }));
            Assert.Equal(HttpStatusCode.OK, customerResponse.StatusCode);
            var customer = await customerResponse.ReadAsAsync<CustomerResponseDTO>();

            var assignResponse = await _client.SendAsync(Authed(HttpMethod.Patch,
                $"/api/customer/{customer!.Id}/workspace/{workSpace!.Id}", token));
            Assert.Equal(HttpStatusCode.OK, assignResponse.StatusCode);

            var orderResponse = await _client.SendAsync(Authed(HttpMethod.Post, "/api/order/create", token,
                new OrderRequestDTO { CustomerId = customer.Id, WorkSpaceId = workSpace.Id, Title = "Wedding Gown" }));

            Assert.Equal(HttpStatusCode.Created, orderResponse.StatusCode);
            var order = await orderResponse.ReadAsAsync<OrderResponseDTO>();
            Assert.Equal("Wedding Gown", order!.Title);
            Assert.Null(order.AssignedToUserId);
        }

        [Fact]
        public async Task CreateOrder_AsTailorInvitedToWorkSpace_ReturnsForbidden()
        {
            var suffix = nameof(CreateOrder_AsTailorInvitedToWorkSpace_ReturnsForbidden);
            var ownerToken = await RegisterAndLoginAsync($"owner_{suffix}");

            var workSpaceResponse = await _client.SendAsync(Authed(HttpMethod.Post, "/api/workspace/create", ownerToken,
                new WorkSpaceRequestDTO { Name = $"Grace Couture {suffix}" }));
            var workSpace = await workSpaceResponse.ReadAsAsync<WorkSpaceResponseDTO>();

            var customerResponse = await _client.SendAsync(Authed(HttpMethod.Post, "/api/customer/create", ownerToken,
                new CustomerRequestDTO { FullName = "Amara Okafor", Phone = $"555-{suffix}" }));
            var customer = await customerResponse.ReadAsAsync<CustomerResponseDTO>();
            await _client.SendAsync(Authed(HttpMethod.Patch, $"/api/customer/{customer!.Id}/workspace/{workSpace!.Id}", ownerToken));

            var tailorEmail = $"tailor_{suffix}@test.com";
            var inviteResponse = await _client.SendAsync(Authed(HttpMethod.Post, "/api/workspace/invite", ownerToken,
                new InvitationRequestDTO
                {
                    WorkSpaceId = workSpace.Id,
                    Email = tailorEmail,
                    Subject = "Join us",
                    Body = "Come work with us",
                    Role = nameof(Role.Tailor)
                }));
            Assert.Equal(HttpStatusCode.OK, inviteResponse.StatusCode);

            // Registering with the invited email auto-joins the workspace (UserService picks up
            // the pending invitation) — this is the same behavior covered by
            // UserServiceTests.RegisterUserAsync_WithPendingInvitation_AutoJoinsWorkSpace, now
            // exercised through the real registration endpoint instead of the service directly.
            var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequestDTO
            {
                FirstName = "Tailor",
                LastName = $"Person_{suffix}",
                UserName = $"tailor_{suffix}",
                Email = tailorEmail,
                Password = "Str0ngPass"
            });
            Assert.Equal(HttpStatusCode.OK, registerResponse.StatusCode);

            var tailorLoginResponse = await _client.PostAsJsonAsync("/api/auth/login",
                new LoginRequestDTO { UserName = $"tailor_{suffix}", Password = "Str0ngPass" });
            var tailorToken = (await tailorLoginResponse.ReadAsAsync<Dictionary<string, string>>())!["token"];

            var orderResponse = await _client.SendAsync(Authed(HttpMethod.Post, "/api/order/create", tailorToken,
                new OrderRequestDTO { CustomerId = customer.Id, WorkSpaceId = workSpace.Id, Title = "Suit" }));

            // A Tailor is a real workspace member but isn't Owner/Assistant, so creating an
            // order must be rejected — this proves EnsureIsOwnerOrAssistantAsync's
            // UnauthorizedAccessException actually reaches the client as a 403 through the
            // whole real pipeline, not just in the service-level unit tests.
            Assert.Equal(HttpStatusCode.Forbidden, orderResponse.StatusCode);
        }

        [Fact]
        public async Task Orders_AreIsolatedBetweenUnrelatedWorkSpaces()
        {
            var suffix = nameof(Orders_AreIsolatedBetweenUnrelatedWorkSpaces);

            async Task<(string Token, int OrderId)> CreateWorkSpaceWithOrderAsync(string who)
            {
                var token = await RegisterAndLoginAsync($"{who}_{suffix}");
                var workSpaceResponse = await _client.SendAsync(Authed(HttpMethod.Post, "/api/workspace/create", token,
                    new WorkSpaceRequestDTO { Name = $"WS {who} {suffix}" }));
                var workSpace = await workSpaceResponse.ReadAsAsync<WorkSpaceResponseDTO>();

                var customerResponse = await _client.SendAsync(Authed(HttpMethod.Post, "/api/customer/create", token,
                    new CustomerRequestDTO { FullName = $"Customer {who}", Phone = $"555-{who}-{suffix}" }));
                var customer = await customerResponse.ReadAsAsync<CustomerResponseDTO>();
                await _client.SendAsync(Authed(HttpMethod.Patch, $"/api/customer/{customer!.Id}/workspace/{workSpace!.Id}", token));

                var orderResponse = await _client.SendAsync(Authed(HttpMethod.Post, "/api/order/create", token,
                    new OrderRequestDTO { CustomerId = customer.Id, WorkSpaceId = workSpace.Id, Title = $"Order {who}" }));
                var order = await orderResponse.ReadAsAsync<OrderResponseDTO>();

                return (token, order!.Id);
            }

            var (tokenA, orderAId) = await CreateWorkSpaceWithOrderAsync("A");
            var (tokenB, orderBId) = await CreateWorkSpaceWithOrderAsync("B");

            // A has no membership at all in B's workspace — reading B's order by id must fail...
            var crossReadResponse = await _client.SendAsync(Authed(HttpMethod.Get, $"/api/order/{orderBId}", tokenA));
            Assert.Equal(HttpStatusCode.NotFound, crossReadResponse.StatusCode);

            // ...and B's order must never show up in A's list, even though both exist in the DB.
            var listResponse = await _client.SendAsync(Authed(HttpMethod.Get, "/api/order/list", tokenA));
            var page = await listResponse.ReadAsAsync<PageResultDTO<OrderResponseDTO>>();
            Assert.DoesNotContain(page!.Items!, o => o.Id == orderBId);

            // Sanity check: A can still read its own order fine.
            var ownReadResponse = await _client.SendAsync(Authed(HttpMethod.Get, $"/api/order/{orderAId}", tokenA));
            Assert.Equal(HttpStatusCode.OK, ownReadResponse.StatusCode);
        }
    }
}
