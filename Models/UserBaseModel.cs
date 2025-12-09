using System.ComponentModel.DataAnnotations;
using ValidationDemo.Enums;
using ValidationDemo.ValidationAttributes;

namespace ValidationDemo.Models
{
    public class UserBaseModel
    {
        public UserBaseModel()
        {
            DateOfBirth = DateTime.Today;
        }
        [Required(ErrorMessage = "Username is required")]
        [StringLength(10, MinimumLength = 3, ErrorMessage = "Username length must be within 3 to 10 characters")]
        [Display(Name = "User Name *")]
        [Alphanumeric(ErrorMessage = "Username can only contain letters and numbers")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Email field must not be empty")]
        [CustomEmail(ErrorMessage = "Please enter a valid email address")]
        [RegularExpression(@"^[^\s@]+@[^\s@]+\.[^\s@]+$",
    ErrorMessage = "Enter a complete email address (e.g., user@example.com).")]
        [Display(Name = "Email Address *")]
        public string? Email { get; set; }
        // Date Picker - Date of Birth
        [Required(ErrorMessage = "Date of Birth is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth *")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Age field is mandatory")]
        [Range(18, 100, ErrorMessage = "Age must be between 18 and 100 years")]
        [Display(Name = "Age *")]
        public int Age { get; set; }

        // Radio Button - Gender
        [Required(ErrorMessage = "Please select your gender")]
        [Display(Name = "Gender *")]
        public string? Gender { get; set; }

        [Required(ErrorMessage = "Phone Number must be filled")]
        [Phone(ErrorMessage = "Invalid mobile number")]
        [Display(Name = "Mobile Number *")]
        [IndianMobile(ErrorMessage = "Please enter a valid 10-digit Indian mobile number")]
        public string? PhoneNumber { get; set; }

        // Dropdown - Country Selection
        [Required(ErrorMessage = "Please select a country")]
        [Display(Name = "Country *")]
        public CountryEnum Country { get; set; }

        [RegularExpression(@"^(https?:\/\/)?(www\.)?([a-zA-Z0-9\-]+\.)+[a-zA-Z]{2,}(\/.*)?$|^(https?:\/\/)?localhost(:\d+)?(\/.*)?$",
            ErrorMessage = "Enter a valid URL")]
        [Display(Name = "Website (Optional)")]
        public string? Website { get; set; }

        // Checkbox - Accept Terms
        //[Range(typeof(bool), "true", "true", ErrorMessage = "You must accept the terms and conditions")]
        //[Display(Name = "Accept Terms and Conditions")]
        //public bool AcceptTerms { get; set; }

        // Checkbox - Newsletter Subscription (Optional)
        [Display(Name = "Subscribe to Newsletter")]
        public bool SubscribeNewsletter { get; set; }
    }

}
