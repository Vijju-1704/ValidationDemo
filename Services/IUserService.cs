using ValidationDemo.Models;
using ValidationDemo.Common;
namespace ValidationDemo.Services
{
    public interface IUserService
    {
        // Registration
        Task<(bool Success, string Message, UserEntity User)> RegisterUserAsync(UserRegistrationModel model);

        // Update
        Task<(bool Success, string Message, UserEntity User)> UpdateUserAsync(EditUserModel model);

        // Get operations
        Task<UserEntity> GetUserByIdAsync(int id);
        Task<IEnumerable<UserEntity>> GetAllActiveUsersAsync();
        Task<IEnumerable<UserEntity>> GetAllDeletedUsersAsync();

        // Delete and Restore
        Task<(bool Success, string Message)> DeleteUserAsync(int id);
        Task<(bool Success, string Message)> RestoreUserAsync(int id);

        // Validation
        Task<(bool IsValid, string ErrorMessage)> ValidateUsernameAsync(string username, int? excludeUserId = null);
        Task<(bool IsValid, string ErrorMessage)> ValidateEmailAsync(string email, int? excludeUserId = null);
        Task<UserEntity?> ValidateUserAsync(string username, string password);

        //ViewComponent related
        Task<int> GetTotalUsersCountAsync();
        Task<int> GetActiveUsersCountAsync();
        Task<int> GetDeletedUsersCountAsync();

        // <summary>
        /// Get user by username
        /// </summary>
        Task<UserEntity?> GetUserByUsernameAsync(string username);

        /// <summary>
        /// Update user lockout status
        /// </summary>
        Task<OperationResult> UpdateUserLockoutAsync(UserEntity user);

        /// <summary>
        /// Reset failed login attempts to 0
        /// </summary>
        Task ResetFailedLoginAttemptsAsync(int userId);

        /// <summary>
        /// Increment failed login attempts and lock if needed
        /// </summary>
        Task IncrementFailedLoginAttemptsAsync(string username);

        /// <summary>
        /// Update last login timestamp and IP address
        /// </summary>
        Task UpdateLastLoginAsync(int userId, string ipAddress);

        /// <summary>
        /// Assign a role to a user (Admin, Manager, User)
        /// </summary>
        Task<OperationResult> AssignRoleAsync(int userId, string roleName);

        /// <summary>
        /// Assign permissions to a user
        /// </summary>
        Task<OperationResult> AssignPermissionsAsync(int userId, string permissions);
        Task UpdateAsync(UserEntity user);

    }
}