using Microsoft.EntityFrameworkCore;
using ValidationDemo.Constants;
using ValidationDemo.Data;
using ValidationDemo.Models;

namespace ValidationDemo.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext DbContect;
        public UserRepository(ApplicationDbContext context)
        {
            DbContect = context;
        }
        // Get user by ID
        public async Task<UserEntity?> GetByIdAsync(int id)
        {
            var user = await DbContect.Users.FindAsync(id);
            if (user == null)
            {
                throw new InvalidOperationException(Messages.RepoUserNotFound);
            }
            return user;
        }
        // Get user by username
        public async Task<UserEntity?> GetByUsernameAsync(string username)
        {
            var user = await DbContect.Users
                .FirstOrDefaultAsync(u => u.Username == username);
            if (user == null)
            {
                throw new InvalidOperationException(Messages.RepoUserByUsernameNotFound);
            }
            return user;
        }
        // Get user by email
        public async Task<UserEntity?> GetByEmailAsync(string email)
        {
            var user = await DbContect.Users
                .FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                throw new InvalidOperationException(Messages.RepoUserByEmailNotFound);
            }
            return user;
        }
        // Get all active users
        public async Task<IEnumerable<UserEntity>> GetAllActiveUsersAsync()
        {
            return await DbContect.Users
                .Where(u => u.IsActive)
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }
        // Get all deleted users
        public async Task<IEnumerable<UserEntity>> GetAllDeletedUsersAsync()
        {
            return await DbContect.Users
                .Where(u => !u.IsActive)
                .OrderByDescending(u => u.DeletedAt)
                .ToListAsync();
        }
        // Check if username exists (excluding specific user ID)
        public async Task<bool> UsernameExistsAsync(string username, int? excludeUserId = null)
        {
            return await DbContect.Users
                            .AnyAsync(u =>
                            u.Username == username &&
                            u.IsActive &&
                            (!excludeUserId.HasValue || u.Id != excludeUserId.Value));
        }
        // Check if email exists (excluding specific user ID)
        public async Task<bool> EmailExistsAsync(string email, int? excludeUserId = null)
        {
            return await DbContect.Users
                            .AnyAsync(u =>
                            u.Email == email &&
                            u.IsActive &&
                            (!excludeUserId.HasValue || u.Id != excludeUserId.Value));
        }
        // Create new user
        public async Task<UserEntity> CreateUserAsync(UserEntity user)
        {
            DbContect.Users.Add(user);
            await DbContect.SaveChangesAsync();
            return user;
        }
        // Update user
        public async Task<UserEntity> UpdateUserAsync(UserEntity user)
        {
            user.UpdatedAt = DateTime.UtcNow;
            DbContect.Users.Update(user);
            await DbContect.SaveChangesAsync();
            return user;
        }
        // Soft delete user (set IsActive = false)
        public async Task<bool> SoftDeleteUserAsync(int id)
        {
            var user = await GetByIdAsync(id);

            if (user == null || !user.IsActive)
            {
                return false;
            }
            user.IsActive = false;
            user.DeletedAt = DateTime.UtcNow;
            DbContect.Users.Update(user);
            await DbContect.SaveChangesAsync();
            return true;
        }
        // Restore deleted user
        public async Task<bool> RestoreUserAsync(int id)
        {
            var user = await GetByIdAsync(id);
            if (user == null || user.IsActive)
            {
                return false;
            }
            user.IsActive = true;
            user.DeletedAt = null;
            user.UpdatedAt = DateTime.UtcNow;
            DbContect.Users.Update(user);
            await DbContect.SaveChangesAsync();
            return true;
        }

        // Save changes to database
        public async Task<bool> SaveChangesAsync()
        {
            return await DbContect.SaveChangesAsync() > 0;
        }
        public Task<bool> UserExistsAsync(EditUserModel model, int id)
        {
            throw new NotImplementedException();
        }
    }
}