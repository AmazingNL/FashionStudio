using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FashionStudio.Api.Models;
using FashionStudio.Api.Services;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace FashionStudio.Api.Tests
{
    public class TokenServiceTests
    {
        private static IConfiguration CreateConfig(string? jwtKey = "unit-test-signing-key-please-ignore") =>
            new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Jwt:Key"] = jwtKey,
                    ["Jwt:Issuer"] = "FashionStudio.Tests",
                    ["Jwt:Audience"] = "FashionStudio.Tests"
                })
                .Build();

        [Fact]
        public void GenerateToken_ValidUser_EmbedsUserIdClaim()
        {
            var service = new TokenService(CreateConfig());
            var user = new User { Id = 42, UserName = "amara" };

            var token = service.GenerateToken(user);

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            var idClaim = jwt.Claims.Single(c => c.Type == ClaimTypes.NameIdentifier);
            Assert.Equal("42", idClaim.Value);
        }

        [Fact]
        public void GenerateToken_UserWithoutUserName_Throws()
        {
            var service = new TokenService(CreateConfig());
            var user = new User { Id = 1, UserName = "" };

            Assert.Throws<InvalidOperationException>(() => service.GenerateToken(user));
        }

        [Fact]
        public void GenerateToken_MissingJwtKey_Throws()
        {
            var service = new TokenService(CreateConfig(jwtKey: null));
            var user = new User { Id = 1, UserName = "amara" };

            Assert.Throws<InvalidOperationException>(() => service.GenerateToken(user));
        }
    }
}
