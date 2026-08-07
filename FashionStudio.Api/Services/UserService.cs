using FashionStudio.Api.Data;
using FashionStudio.Api.Models;
using BCrypt.Net;
using FashionStudio.Api.Interfaces;

namespace FashionStudio.Api.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext? _context;
        public UserService(AppDbContext? context)
        {
            _context = context;
        }

        public async Task<User> IUserService.VerifyPasswordAsync(string userName, string password)
        {
            User? user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
            if (user == null)
            {
                Invalid username or password.
                    throw new UnauthorizedAccessException("Invalid username or password.");
            }
            // Verify the password using BCrypt
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
            if (!isPasswordValid)
            {
                throw new UnauthorizedAccessException("Invalid username or password.");
            }
            return user);
        }

        public async Task<User> CreateUserAsync(User user, string password)
        {
            try
            {
                user.Password = HashPassword(password);
                user.JoinedAt = DateTime.UtcNow;
                user.IsActive = true;
                _context!.Users.Add(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during user creation
                throw new InvalidOperationException("An error occurred while creating the user.", ex);
            }
        }

        Task<User> IUserService.GetUserByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        Task<User> IUserService.GetUserByIdAsync(int userId)
        {
            throw new NotImplementedException();
        }

        private string HashPassword(string password)
        {
            // Using BCrypt to hash the password securely
            try
            {
                if (ValidatePass(password))
                    password = BCrypt.Net.BCrypt.HashPassword(password);
                return password;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An error occurred while hashing the password.", ex);
            }

        }

        private bool ValidatePass(string password)
        {
            // Implement a secure password hashing mechanism
            password = password.Trim();
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

