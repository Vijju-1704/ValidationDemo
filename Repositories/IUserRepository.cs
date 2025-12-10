using ValidationDemo.Models;

namespace ValidationDemo.Repositories
{
    public interface IUserRepository : IGenericRepository<UserEntity>
    {
        // Get operations (specific to User)
        Task<UserEntity?> GetByUsernameAsync(string username);
        Task<UserEntity?> GetByEmailAsync(string email);
        Task<UserEntity?> GetActiveUserByUsernameAsync(string username);
        Task<IEnumerable<UserEntity>> GetAllActiveUsersAsync();
        Task<IEnumerable<UserEntity>> GetAllDeletedUsersAsync();

        // Check operations (specific to User)
        Task<bool> UsernameExistsAsync(string username, int? excludeUserId = null);
        Task<bool> EmailExistsAsync(string email, int? excludeUserId = null);

        // CRUD operations (specific to User)
        Task<UserEntity> CreateUserAsync(UserEntity user);
        Task<UserEntity> UpdateUserAsync(UserEntity user);
        Task<bool> SoftDeleteUserAsync(int id);
        Task<bool> RestoreUserAsync(int id);

        // Statistics (ViewComponent related)
        Task<int> GetTotalCountAsync();
        Task<int> GetActiveCountAsync();
        Task<int> GetDeletedCountAsync();
    }
}
