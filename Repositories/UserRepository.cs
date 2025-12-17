using Microsoft.EntityFrameworkCore;
using ValidationDemo.Constants;
using ValidationDemo.Data;
using ValidationDemo.Models;

namespace ValidationDemo.Repositories
{
    public class UserRepository : GenericRepository<UserEntity>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        // Override GetByIdAsync to throw exception if not found
        public override async Task<UserEntity?> GetByIdAsync(int id)
        {
            var user = await base.GetByIdAsync(id);
            if (user == null)
            {
                throw new InvalidOperationException(Messages.RepoUserNotFound);
            }
            return user;
        }

        // Get user by username
        public async Task<UserEntity?> GetByUsernameAsync(string username)
        {
            var user = await FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                throw new InvalidOperationException(Messages.RepoUserByUsernameNotFound);
            }
            return user;
        }

        // Get user by email
        public async Task<UserEntity?> GetByEmailAsync(string email)
        {
            var user = await FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                throw new InvalidOperationException(Messages.RepoUserByEmailNotFound);
            }
            return user;
        }

        // Get all active users
        public async Task<IEnumerable<UserEntity>> GetAllActiveUsersAsync()
        {
            return await DbContext.Users
                .Where(u => u.IsActive)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        // Get all deleted users
        public async Task<IEnumerable<UserEntity>> GetAllDeletedUsersAsync()
        {
            return await DbContext.Users
                .Where(u => !u.IsActive)
                .OrderByDescending(u => u.DeletedAt)
                .ToListAsync();
        }

        // Check if username exists (excluding specific user ID)
        public async Task<bool> UsernameExistsAsync(string username, int? excludeUserId = null)
        {
            return await AnyAsync(u =>
                u.Username == username &&
                u.IsActive &&
                (!excludeUserId.HasValue || u.Id != excludeUserId.Value));
        }

        // Check if email exists (excluding specific user ID)
        public async Task<bool> EmailExistsAsync(string email, int? excludeUserId = null)
        {
            return await AnyAsync(u =>
                u.Email == email &&
                u.IsActive &&
                (!excludeUserId.HasValue || u.Id != excludeUserId.Value));
        }

        // Get active user by username for authentication
        public async Task<UserEntity?> GetActiveUserByUsernameAsync(string username)
        {
            return await FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
        }

        // Create new user
        public async Task<UserEntity> CreateUserAsync(UserEntity user)
        {
            await AddAsync(user);
            await DbContext.SaveChangesAsync();
            return user;
        }

        // Update user
        public async Task<UserEntity> UpdateUserAsync(UserEntity user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            Update(user);
            await DbContext.SaveChangesAsync();
            return user;
        }

        // Soft delete user (set IsActive = false)
        public async Task<bool> SoftDeleteUserAsync(int id)
        {
            var user = await DbSet.FindAsync(id);

            if (user == null || !user.IsActive)
            {
                return false;
            }

            user.IsActive = false;
            user.DeletedAt = DateTime.UtcNow;
            Update(user);
            await DbContext.SaveChangesAsync();
            return true;
        }

        // Restore deleted user
        public async Task<bool> RestoreUserAsync(int id)
        {
            var user = await DbSet.FindAsync(id);

            if (user == null || user.IsActive)
            {
                return false;
            }

            user.IsActive = true;
            user.DeletedAt = null;
            user.UpdatedAt = DateTime.UtcNow;
            Update(user);
            await DbContext.SaveChangesAsync();
            return true;
        }

        // Statistics methods
        public async Task<int> GetTotalCountAsync()
        {
            return await CountAsync();
        }

        public async Task<int> GetActiveCountAsync()
        {
            return await CountAsync(u => u.IsActive);
        }

        public async Task<int> GetDeletedCountAsync()
        {
            return await CountAsync(u => !u.IsActive);
        }
    }
}
