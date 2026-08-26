using FashionStudio.Api.Models;

namespace FashionStudio.Api.Interfaces
{
    public interface ITokenService
    {
        public string GenerateToken(User user);
    }
}