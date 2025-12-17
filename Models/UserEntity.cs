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
        [MaxLength(50)]
        public string Role { get; set; } = "User";
    }
}