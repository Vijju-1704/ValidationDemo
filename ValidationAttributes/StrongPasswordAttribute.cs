using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace ValidationDemo.ValidationAttributes
{
    public class StrongPasswordAttribute : ValidationAttribute,IClientModelValidator
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success;
            }

            string? password = value.ToString();
            if (password == null)
            {
                return ValidationResult.Success;
            }

            bool hasUpperCase = password.Any(char.IsUpper);
            bool hasLowerCase = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecialChar = password.Any(ch => !char.IsLetterOrDigit(ch));

            if (hasUpperCase && hasLowerCase && hasDigit && hasSpecialChar)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(
                ErrorMessage ?? "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character"
            );
        }
        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            context.Attributes.Add("data-val", "true");
            context.Attributes.Add("data-val-strongpassword",
              ErrorMessage ?? "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character");

            //MergeAttribute(context.Attributes, "data-val", "true");
            //MergeAttribute(context.Attributes, "data-val-alphanumeric",
            //    ErrorMessage ?? "Only alphanumeric characters are allowed");
        }
    }
}


