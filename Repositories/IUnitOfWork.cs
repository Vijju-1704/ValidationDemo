namespace ValidationDemo.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository Users { get; }
        // Add other repositories here as your application grows
        // IProductRepository Products { get; }
        // IOrderRepository Orders { get; }

        Task<int> SaveChangesAsync();
        Task<bool> SaveAsync();
    }
}