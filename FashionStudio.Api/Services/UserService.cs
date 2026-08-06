using FashionStudio.Api.Data;
using FashionStudio.Api.Models;
using BCrypt.Net;

namespace FashionStudio.Api.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext? _context;
        public UserService(AppDbContext? context)
        {
            _context = context;
        }

        Task<bool> IUserService.CheckPasswordAsync(User user, string password)
        {
            throw new NotImplementedException();
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
            // Implement a secure password hashing mechanism here
            // For example, you can use BCrypt or PBKDF2
            try
            {
                if (ValidatePass(password))
                    password = BCrypt.Net.BCrypt.HashPassword(password);
                return password;
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during hashing
                throw new InvalidOperationException("An error occurred while hashing the password.", ex);
            }

        }

        private bool ValidatePass(string password)
        {
            // Implement a secure password hashing mechanism here
            // For example, you can use BCrypt or PBKDF2
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

