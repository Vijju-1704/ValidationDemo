using ValidationDemo.Data;

namespace ValidationDemo.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext DbContext;
        private IUserRepository? User;

        public UnitOfWork(ApplicationDbContext context)
        {
            DbContext = context;
        }

        public IUserRepository Users
        {
            get
            {
                User ??= new UserRepository(DbContext);
                return User;
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await DbContext.SaveChangesAsync();
        }

        public async Task<bool> SaveAsync()
        {
            return await DbContext.SaveChangesAsync() > 0;
        }

        public void Dispose()
        {
            DbContext.Dispose();
        }
    }
}
