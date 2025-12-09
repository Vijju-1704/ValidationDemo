using Microsoft.EntityFrameworkCore;
using ValidationDemo.Models;

namespace ValidationDemo.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Using UserEntity for database, UserRegistrationModel stays unchanged
        public DbSet<UserEntity> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure UserEntity
            modelBuilder.Entity<UserEntity>(entity =>
            {
                // Create unique index on Username
                entity.HasIndex(u => u.Username)
                    .IsUnique()
                    .HasDatabaseName("IX_Users_Username");

                // Create unique index on Email
                entity.HasIndex(u => u.Email)
                    .IsUnique()
                    .HasDatabaseName("IX_Users_Email");

                // Set default value for CreatedAt
                entity.Property(u => u.CreatedAt)
                    .HasDefaultValueSql("GETUTCDATE()");

                entity.Property(u => u.Country)
                    .HasConversion<string>();
                //entity.Property(u => u.IsActive)
                //       .HasDefaultValue(true).ValueGeneratedOnAdd();

            });
        }

    }
}