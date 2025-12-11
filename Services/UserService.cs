using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Text;
using ValidationDemo.Common;
using ValidationDemo.Constants;
using ValidationDemo.Models;
using ValidationDemo.Repositories;
using ValidationDemo.Common;

namespace ValidationDemo.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(IUserRepository UserRepository, IPasswordHasher passwordHasher)
        {
            this._userRepository = UserRepository;
            _passwordHasher = passwordHasher;
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
            if (await _userRepository.UsernameExistsAsync(model.Username ?? string.Empty))
            {
                return (false, Messages.UsernameExists, null!);
            }
            // Validate email is not null or whitespace
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                return (false, Messages.EmailRequired, null!);
            }
            // Check if email exists
            if (await _userRepository.EmailExistsAsync(model.Email!))
            {
                return (false, Messages.EmailExists, null!);
            }
            // Validate password is not null or whitespace
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                return (false, Messages.PasswordRequired, null!);
            }

            var hashedPassword = _passwordHasher.HashPassword(model.Password);
            // Create user entity with ALL new fields
            var user = new UserEntity
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = hashedPassword, // Store hashed password
                DateOfBirth = model.DateOfBirth,
                Age = model.Age,
                PhoneNumber = model.PhoneNumber,
                Website = model.Website,
                Gender = model.Gender,
                Country = model.Country,
                SubscribeNewsletter = model.SubscribeNewsletter,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                Role = "User", // Default role
                Department = null,
                Permissions = "ViewProfile,EditProfile"
            };
            // Save to database
            var createdUser = await _userRepository.CreateUserAsync(user);
            return (true, string.Format(Messages.UserRegistered, model.Username), createdUser);
        }

        // Update user
        public async Task<(bool Success, string Message, UserEntity User)> UpdateUserAsync(EditUserModel model)
        {
            // Get user
            var user = await _userRepository.GetByIdAsync(model.Id);
            if (user?.IsActive != true)
            {
                return (false, Messages.UserNotFound, null!);
            }

            // Check username availability (excluding current user)
            if (string.IsNullOrWhiteSpace(model.Username))
            {
                return (false, Messages.UsernameRequired, null!);
            }
            if (await _userRepository.UsernameExistsAsync(model.Username!, model.Id))
            {
                return (false, Messages.UsernameExists, null!);
            }
            // Check email availability (excluding current user)
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                return (false, Messages.EmailRequired, null!);
            }
            if (await _userRepository.EmailExistsAsync(model.Email!, model.Id))
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
            var updatedUser = await _userRepository.UpdateUserAsync(user);
            return (true, string.Format(Messages.UserUpdated, model.Username), updatedUser);
        }

        // Get user by ID
        public async Task<UserEntity> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                throw new InvalidOperationException(Messages.UserNotFound);
            }
            return user;
        }

        // Get all active users
        public async Task<IEnumerable<UserEntity>> GetAllActiveUsersAsync()
        {
            return await _userRepository.GetAllActiveUsersAsync();
        }

        // Get all deleted users
        public async Task<IEnumerable<UserEntity>> GetAllDeletedUsersAsync()
        {
            return await _userRepository.GetAllDeletedUsersAsync();
        }

        // Soft delete user
        public async Task<(bool Success, string Message)> DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return (false, Messages.UserNotFound);
            }
            if (!user.IsActive)
            {
                return (false, Messages.UserAlreadyDeleted);
            }
            var success = await _userRepository.SoftDeleteUserAsync(id);
            if (success)
            {
                return (true, string.Format(Messages.UserDeleted, user.Username));
            }
            return (false, Messages.FailedToDeleteUser);
        }

        // Restore deleted user
        public async Task<(bool Success, string Message)> RestoreUserAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return (false, Messages.UserNotFound);
            }
            if (user.IsActive)
            {
                return (false, Messages.UserRestored);
            }
            var success = await _userRepository.RestoreUserAsync(id);
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
            if (await _userRepository.UsernameExistsAsync(username, excludeUserId))
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
            if (await _userRepository.EmailExistsAsync(email, excludeUserId))
            {
                return (false, Messages.EmailExists);
            }
            return (true, Messages.Empty);
        }

        // Validate user credentials for login
        public async Task<UserEntity?> ValidateUserAsync(string username, string password)
        {
            // Get active user by username from repository
            var user = await _userRepository.GetActiveUserByUsernameAsync(username);

            if (user == null)
            {
                return null;
            }

            // In production, you should hash the password and compare hashes
            // For now, we'll do a simple comparison (THIS IS NOT SECURE!)
            // TODO: Implement proper password hashing (BCrypt, PBKDF2, etc.)
            if (!_passwordHasher.VerifyPassword(password, user.PasswordHash))
                return null;

            return user;
        }

        public async Task<UserEntity?> GetUserByUsernameAsync(string username)
        {
            return await _userRepository.GetByUsernameAsync(username);
        }

        public async Task<OperationResult> UpdateUserLockoutAsync(UserEntity user)
        {
            await _userRepository.UpdateAsync(user);

            return new OperationResult
            {
                Success = true,
                Message = "User lockout status updated"
            };
        }

        public async Task ResetFailedLoginAttemptsAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user != null)
            {
                user.FailedLoginAttempts = 0;
                user.LockoutEnd = null;
                await _userRepository.UpdateAsync(user);
            }
        }

        public async Task IncrementFailedLoginAttemptsAsync(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user != null)
            {
                user.FailedLoginAttempts++;

                // Lock account after 5 failed attempts for 15 minutes
                if (user.FailedLoginAttempts >= 5)
                {
                    user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
                }

                await _userRepository.UpdateAsync(user);
            }
        }

        public async Task UpdateLastLoginAsync(int userId, string ipAddress)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user != null)
            {
                user.LastLoginAt = DateTime.UtcNow;
                user.LastLoginIp = ipAddress;
                await _userRepository.UpdateAsync(user);
            }
        }

        public async Task<OperationResult> AssignRoleAsync(int userId, string roleName)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            // Validate role name
            var validRoles = new[] { "Admin", "Manager", "User" };
            if (!validRoles.Contains(roleName))
            {
                return new OperationResult
                {
                    Success = false,
                    Message = "Invalid role name"
                };
            }

            user.Role = roleName;
            await _userRepository.UpdateAsync(user);

            return new OperationResult
            {
                Success = true,
                Message = $"Role '{roleName}' assigned successfully"
            };
        }


        public async Task<OperationResult> AssignPermissionsAsync(int userId, string permissions)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return new OperationResult
                {
                    Success = false,
                    Message = "User not found"
                };
            }

            user.Permissions = permissions; // Example: "ViewReports,EditUsers,DeleteUsers"
            await _userRepository.UpdateAsync(user);

            return new OperationResult
            {
                Success = true,
                Message = "Permissions assigned successfully"
            };
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

        // ViewComponent related methods
        public async Task<int> GetTotalUsersCountAsync()
        {
            return await _userRepository.GetTotalCountAsync();
        }

        public async Task<int> GetActiveUsersCountAsync()
        {
            return await _userRepository.GetActiveCountAsync();
        }

        public async Task<int> GetDeletedUsersCountAsync()
        {
            return await _userRepository.GetDeletedCountAsync();
        }
        public async Task UpdateAsync(UserEntity user)
        {
            await _userRepository.UpdateAsync(user);
        }
    }
}