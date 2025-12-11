using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ValidationDemo.Enums;

namespace ValidationDemo.Models
{
    [Table("Users")]
    public class UserEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Username { get; set; }

        [Required]
        [MaxLength(100)]
        public string? Email { get; set; }

        [Required]
        [MaxLength(255)]
        public string? PasswordHash { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public int Age { get; set; }

        [Required]
        [MaxLength(10)]
        public string? Gender { get; set; }

        [Required]
        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [Required]
        [MaxLength(100)]
        public CountryEnum Country { get; set; }

        [MaxLength(200)]
        public string? Website { get; set; }

        
        //public bool AcceptTerms { get; set; }

        public bool SubscribeNewsletter { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [Required]
        public bool IsActive { get; internal set; } = true;

        public DateTime? DeletedAt { get; set; }

        [Required]
        [StringLength(50)]
        public string Role { get; set; } = "User"; // Default role

        /// <summary>
        /// Department the user belongs to
        /// </summary>
        [StringLength(100)]
        public string? Department { get; set; }

        /// <summary>
        /// Comma-separated permissions: ViewReports,EditUsers,DeleteUsers
        /// </summary>
        public string? Permissions { get; set; }

        /// <summary>
        /// Number of consecutive failed login attempts
        /// </summary>
        public int FailedLoginAttempts { get; set; } = 0;

        /// <summary>
        /// When the account lockout expires (null = not locked)
        /// </summary>
        public DateTime? LockoutEnd { get; set; }

        /// <summary>
        /// Checks if account is currently locked out
        /// </summary>
        public bool IsLockedOut => LockoutEnd.HasValue && LockoutEnd.Value > DateTime.UtcNow;

        /// <summary>
        /// Last successful login timestamp
        /// </summary>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// IP address of last login
        /// </summary>
        [StringLength(50)]
        public string? LastLoginIp { get; set; }
    }
}