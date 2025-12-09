using ValidationDemo.Models;
using ValidationDemo.Repositories;
using System.Security.Cryptography;
using System.Text;
using ValidationDemo.Constants;

namespace ValidationDemo.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository UserRepository;
        public UserService(IUserRepository UserRepository)
        {
            this.UserRepository = UserRepository;
        }
        // Register new user
        public async Task<(bool Success, string Message, UserEntity User)> RegisterUserAsync(UserRegistrationModel model)
        {
            // Validate username is not null or whitespace
            if (string.IsNullOrWhiteSpace(model.Username))
            {
                return (false, Messages.UsernameRequired, null!);
            }
            // Check if username exists
            if (await UserRepository.UsernameExistsAsync(model.Username ?? string.Empty))
            {
                return (false, Messages.UsernameExists, null!);
            }
            // Validate email is not null or whitespace
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                return (false, Messages.EmailRequired, null!);
            }
            // Check if email exists
            if (await UserRepository.EmailExistsAsync(model.Email!))
            {
                return (false, Messages.EmailExists, null!);
            }
            // Validate password is not null or whitespace
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                return (false, Messages.PasswordRequired, null!);
            }
            // Create user entity with ALL new fields
            var user = new UserEntity
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = HashPassword(model.Password!),
                DateOfBirth = model.DateOfBirth,
                Age = model.Age,
                Gender = model.Gender,
                PhoneNumber = model.PhoneNumber,
                Country = (Enums.CountryEnum)model.Country,
                Website = model.Website ?? string.Empty,
                //AcceptTerms = model.AcceptTerms,
                SubscribeNewsletter = model.SubscribeNewsletter,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            // Save to database
            var createdUser = await UserRepository.CreateUserAsync(user);
            return (true, string.Format(Messages.UserRegistered, model.Username), createdUser);
        }
        // Update user
        public async Task<(bool Success, string Message, UserEntity User)> UpdateUserAsync(EditUserModel model)
        {
            // Get user
            var user = await UserRepository.GetByIdAsync(model.Id);
            if (user?.IsActive != true)
            {
                return (false, Messages.UserNotFound, null!);
            }

            // Check username availability (excluding current user)
            if (string.IsNullOrWhiteSpace(model.Username))
            {
                return (false, Messages.UsernameRequired, null!);
            }
            if (await UserRepository.UsernameExistsAsync(model.Username!, model.Id))
            {
                return (false, Messages.UsernameExists, null!);
            }
            // Check email availability (excluding current user)
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                return (false, Messages.EmailRequired, null!);
            }
            if (await UserRepository.EmailExistsAsync(model.Email!, model.Id))
            {
                return (false, Messages.EmailExists, null!);
            }
            // Update ALL properties including new fields
            user.Username = model.Username;
            user.Email = model.Email;
            user.DateOfBirth = model.DateOfBirth;
            user.Age = model.Age;
            user.Gender = model.Gender;
            user.PhoneNumber = model.PhoneNumber;
            user.Country = (Enums.CountryEnum)model.Country;
            user.Website = model.Website ?? string.Empty;
            user.SubscribeNewsletter = model.SubscribeNewsletter;
            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                user.PasswordHash = HashPassword(model.NewPassword);
            }
            // Save changes
            var updatedUser = await UserRepository.UpdateUserAsync(user);
            return (true, string.Format(Messages.UserUpdated, model.Username), updatedUser);
        }
        // Get user by ID
        public async Task<UserEntity> GetUserByIdAsync(int id)
        {
            var user = await UserRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new InvalidOperationException(Messages.UserNotFound);
            }
            return user;
        }
        // Get all active users
        public async Task<IEnumerable<UserEntity>> GetAllActiveUsersAsync()
        {
            return await UserRepository.GetAllActiveUsersAsync();
        }
        // Get all deleted users
        public async Task<IEnumerable<UserEntity>> GetAllDeletedUsersAsync()
        {
            return await UserRepository.GetAllDeletedUsersAsync();
        }
        // Soft delete user
        public async Task<(bool Success, string Message)> DeleteUserAsync(int id)
        {
            var user = await UserRepository.GetByIdAsync(id);
            if (user == null)
            {
                return (false, Messages.UserNotFound);
            }
            if (!user.IsActive)
            {
                return (false, Messages.UserAlreadyDeleted);
            }
            var success = await UserRepository.SoftDeleteUserAsync(id);
            if (success)
            {
                return (true, string.Format(Messages.UserDeleted, user.Username));
            }
            return (false, Messages.FailedToDeleteUser);
        }
        // Restore deleted user
        public async Task<(bool Success, string Message)> RestoreUserAsync(int id)
        {
            var user = await UserRepository.GetByIdAsync(id);
            if (user == null)
            {
                return (false, Messages.UserNotFound);
            }
            if (user.IsActive)
            {
                return (false, Messages.UserRestored);
            }
            var success = await UserRepository.RestoreUserAsync(id);
            if (success)
            {
                return (true, string.Format(Messages.UserRestored, user.Username));
            }
            return (false, Messages.FailedToRestoreUser);
        }
        // Validate username
        public async Task<(bool IsValid, string ErrorMessage)> ValidateUsernameAsync(string username, int? excludeUserId = null)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return (false, Messages.UsernameRequired);
            }
            if (await UserRepository.UsernameExistsAsync(username, excludeUserId))
            {
                return (false, Messages.UsernameExists);
            }
            return (true, Messages.Empty);
        }
        // Validate email
        public async Task<(bool IsValid, string ErrorMessage)> ValidateEmailAsync(string email, int? excludeUserId = null)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return (false, Messages.EmailRequired);
            }
            if (await UserRepository.EmailExistsAsync(email, excludeUserId))
            {
                return (false, Messages.EmailExists);
            }
            return (true, Messages.Empty);
        }
        // Helper method to hash password
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
