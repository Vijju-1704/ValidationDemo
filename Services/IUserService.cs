using ValidationDemo.Models;

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


        //Authentication
        Task<(bool Success, string Message)> CreateAdminUserAsync(string username, string email, string password);
    }
}