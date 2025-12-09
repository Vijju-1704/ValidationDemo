using ValidationDemo.Models;

namespace ValidationDemo.Repositories
{
    public interface IUserRepository
    {
        // Get operations
        Task<UserEntity?> GetByIdAsync(int id);
        Task<UserEntity?> GetByUsernameAsync(string username);
        Task<UserEntity?> GetByEmailAsync(string email);
        Task<UserEntity?> GetActiveUserByUsernameAsync(string username);
        Task<IEnumerable<UserEntity>> GetAllActiveUsersAsync();
        Task<IEnumerable<UserEntity>> GetAllDeletedUsersAsync();

        // Check operations
        Task<bool> UsernameExistsAsync(string username, int? excludeUserId = null);
        Task<bool> EmailExistsAsync(string email, int? excludeUserId = null);

        // CRUD operations
        Task<UserEntity> CreateUserAsync(UserEntity user);
        Task<UserEntity> UpdateUserAsync(UserEntity user);
        Task<bool> SoftDeleteUserAsync(int id);
        Task<bool> RestoreUserAsync(int id);

        // Save changes
        Task<bool> SaveChangesAsync();
        Task<bool> UserExistsAsync(EditUserModel model, int id);
    }
}