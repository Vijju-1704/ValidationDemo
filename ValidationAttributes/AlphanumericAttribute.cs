using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace ValidationDemo.ValidationAttributes
{
    public class AlphanumericAttribute : ValidationAttribute,IClientModelValidator
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return ValidationResult.Success;
            }   

            string? stringValue = value.ToString();

            if (!string.IsNullOrEmpty(stringValue) && stringValue.All(char.IsLetterOrDigit))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage ?? "Only alphanumeric characters are allowed");
        }
        public void AddValidation(ClientModelValidationContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            //context.Attributes.Add("data-val", "true");
            //context.Attributes.Add("data-val-alphanumeric",
            //  ErrorMessage ?? "Only alphanumeric characters are allowed");

            MergeAttribute(context.Attributes, "data-val", "true");
            MergeAttribute(context.Attributes, "data-val-alphanumeric",
                ErrorMessage ?? "Only alphanumeric characters are allowed");
        }

        private bool MergeAttribute(IDictionary<string, string> attributes, string key, string value)
        {
            if (attributes.ContainsKey(key))
            {
                return false;
            }

            attributes.Add(key, value);
            return true;
        }
    }
}
