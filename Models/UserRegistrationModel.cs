using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ValidationDemo.Enums;
using ValidationDemo.ValidationAttributes;

namespace ValidationDemo.Models
{
    public class UserRegistrationModel:UserBaseModel
    {
        [Required(ErrorMessage = "Password is required")]
        [StringLength(12, MinimumLength = 6, ErrorMessage = "Password must be atleast 6 characters")]
        [DataType(DataType.Password)]
        [Display(Name = "Password *")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Confirm Password field is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Confirm Password and Password must be same")]
        [Display(Name = "Confirm Password *")]
        public string? ConfirmPassword { get; set; }
        public bool? IsActive { get; set; } = true;
        public DateTime? DeletedAt { get; set; }
    }
}