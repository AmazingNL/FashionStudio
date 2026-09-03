using System.Reflection;
using FashionStudio.Api.Data;
using FashionStudio.Api.DTOs;
using FashionStudio.Api.Interfaces;
using FashionStudio.Api.Models;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace FashionStudio.Api.Tests
{
    public static class TestHelpers
    {
        public static AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        public static IMapper CreateMapper()
        {
            var config = new TypeAdapterConfig();
            config.Scan(typeof(Order).Assembly);
            return new Mapper(config);
        }
    }

    // Only WorkSpaceService's authorization checks are exercised in these tests;
    // the user-lookup methods below are never reached from that code path.
    public class NotImplementedUserService : IUserService
    {
        public Task<User> GetUserByIdAsync(int userId) => throw new NotImplementedException();
        public Task<User> GetUserByEmailAsync(string email) => throw new NotImplementedException();
        public Task<PageResultDTO<UserResponseDTO>> GetAllUsersAsync(QueryParam queryParam, CancellationToken cancellation) => throw new NotImplementedException();
        public Task<UserResponseDTO> RegisterUserAsync(RegisterRequestDTO register) => throw new NotImplementedException();
        public Task<User> VerifyPasswordAsync(LoginRequestDTO request) => throw new NotImplementedException();
    }
}
