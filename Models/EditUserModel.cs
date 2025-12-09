using System.ComponentModel.DataAnnotations;
using ValidationDemo.Enums;
using ValidationDemo.ValidationAttributes;

namespace ValidationDemo.Models
{
    public class EditUserModel: UserBaseModel
    {
             public int Id { get; set; }


        // Password change fields (optional - leave blank to keep current password)
        [DataType(DataType.Password)]
        [Display(Name = "New Password (leave blank to keep current)")]
        [StringLength(12, MinimumLength = 6, ErrorMessage = "Password must be atleast 6 characters")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords must match")]
        [Display(Name = "Confirm New Password")]
        public string? ConfirmNewPassword { get; set; }
    }
}