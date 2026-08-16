using FashionStudio.Api.Models;
using FashionStudio.Api.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using FashionStudio.Api.Constants;

namespace FashionStudio.Api.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateToken(User user)
        {
            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                throw new InvalidOperationException("User must have a username.");
            }
            Claim [] claims = new Claim[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()
                    ),

                new Claim(
                    ClaimTypes.Name,
                    user.UserName
                    ?? string.Empty
                    ),

                new Claim(
                    CustomClaimTypes.WorkspaceId,
                    user.WorkSpaceId.ToString()
                    ?? string.Empty
                    ),
                
                new Claim(
                    CustomClaimTypes.Role,
                    user.Role.ToString()
                    )

            };

            var jwtKey = _configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT key is not configured.");

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}