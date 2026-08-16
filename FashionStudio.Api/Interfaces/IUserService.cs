using FashionStudio.Api.Models;
using FashionStudio.Api.DTOs;

namespace FashionStudio.Api.Interfaces 
{
    public interface IUserService
    {
        public Task<User> GetUserByIdAsync(int userId);
        public Task<User> GetUserByEmailAsync(string email);
        public Task<PageResultDTO<UserResponseDTO>> GetAllUsersAsync(QueryParam queryParam, CancellationToken cancellation);
        public Task<UserResponseDTO> RegisterUserAsync(RegisterRequestDTO register);
        public Task<User> VerifyPasswordAsync(LoginRequestDTO request);
    }
}
 