using FashionStudio.Api.Data;
using FashionStudio.Api.Models;
using FashionStudio.Api.Interfaces;
using Microsoft.EntityFrameworkCore;
using FashionStudio.Api.DTOs;
using FashionStudio.Api.Extensions;
using FashionStudio.Api.Exceptions;
using MapsterMapper;
using Mapster;


namespace FashionStudio.Api.Services
{
    public class UserService : IUserService 
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public UserService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<UserResponseDTO> RegisterUserAsync(RegisterRequestDTO request)
        {

            var user = _mapper.Map<User>(request);
            user.Password = HashPassword(request.Password);

           var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUser != null) throw new ConflictException("A user with this email already exist");

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var pendingInvitations = await _context.WorkSpaceInvitations
                .Where(i => i.Email == user.Email && i.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();
            foreach (var invitation in pendingInvitations)
            {
                await _context.WorkSpaceMemberships.AddAsync(new WorkSpaceMembership
                {
                    User = user,
                    WorkSpaceId = invitation.WorkSpaceId,
                    Role = invitation.Role,
                });
                _context.WorkSpaceInvitations.Remove(invitation);
            }
            if (pendingInvitations.Count > 0)
            {
                await _context.SaveChangesAsync();
            }

            return _mapper.Map<UserResponseDTO>(user);
        }

        public async Task<User> VerifyPasswordAsync(LoginRequestDTO request)
        {
            if (_context == null)
                throw new InvalidOperationException("Database context is not available.");

            User? user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid username or password.");

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.Password);
            if (!isPasswordValid)
                throw new UnauthorizedAccessException("Invalid username or password.");

            return user;
        }

        public async Task<PageResultDTO<UserResponseDTO>> GetAllUsersAsync(QueryParam queryParam, CancellationToken cancellation)
        {

            var pageDto = await _context.Users
                .ProjectToType<UserResponseDTO>()
                .SearchByAttributes(queryParam.SearchTerm)
                .OrderByProperty(queryParam.SortBy, queryParam.IsDescending)
                .ToPagedListAsync(queryParam, cancellation);
            return pageDto;

        }

        public async Task<User> GetUserByIdAsync(int userId)
        {
            if (_context == null)
                throw new InvalidOperationException("Database context is not available.");

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found.");
            return user;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            if (_context == null)
                throw new InvalidOperationException("Database context is not available.");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
                throw new KeyNotFoundException("User not found.");
            return user;
        }

        private string HashPassword(string password)
        {
            try
            {
                if (ValidatePass(password))
                    return BCrypt.Net.BCrypt.HashPassword(password);
                // ValidatePass will throw on invalid password, but return original as fallback
                return password;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while hashing the password.", ex);
            }
        }

        private bool ValidatePass(string password)
        {
            password = password?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException("Password cannot be null or empty.", nameof(password));
            }
            if (password.Length < 6)
                throw new ArgumentException("Password must be at least 6 characters long.", nameof(password));
            if (!password.Any(char.IsUpper))
                throw new ArgumentException("Password must contain at least one uppercase letter.", nameof(password));
            if (!password.Any(char.IsLower))
                throw new ArgumentException("Password must contain at least one lowercase letter.", nameof(password));
            if (!password.Any(char.IsDigit))
                throw new ArgumentException("Password must contain at least one digit.", nameof(password));
            return true;
        }
    }
}

