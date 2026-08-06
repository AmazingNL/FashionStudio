using FashionStudio.Api.Models;

namespace FashionStudio.Api.Services
{
    public interface IUserService
    {
            public Task<User> GetUserByIdAsync(int userId);
            public Task<User> GetUserByEmailAsync(string email);
            public Task<User> CreateUserAsync(User user, string password);
            public Task<bool> CheckPasswordAsync(User user, string password);
    }
}
