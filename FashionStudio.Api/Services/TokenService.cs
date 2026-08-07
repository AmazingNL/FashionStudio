using FashionStudio.Api.Models;
using FashionStudio.Api.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

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
            Claim [] claims = new Claim[]
            {
                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.Id.ToString()
                    ),

                new Claim(
                    ClaimTypes.Name,
                    user.UserName
                    ),

                new Claim(
                    CustomClaimTypes.WorkSpaceId,
                    user.WorkSpaceId.ToString()
                    ),
                
                new Claim(
                    CustomClaimTypes.Role,
                    user.Role.ToString()
                    )

            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds
                );
        }

    }
}