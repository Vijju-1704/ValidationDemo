using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class IndianMobileAttribute : ValidationAttribute, IClientModelValidator
{
    private const string MobilePattern = @"^[6-9]\d{9}$";

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return ValidationResult.Success;
        }

        string? phone = value?.ToString();
        if (string.IsNullOrEmpty(phone))
        {
            return ValidationResult.Success;
        }

        // Remove spaces, dashes, parentheses
        phone = phone.Replace(" ", "")
            .Replace("-", "")
            .Replace("(", "")
            .Replace(")", "");

        // Check if exactly 10 digits
        if (phone.Length != 10)
        {
            return new ValidationResult($"Mobile number must be exactly 10 digits (you entered {phone.Length} digits)");
        }

        // Check if starts with 6-9
        if (!char.IsDigit(phone[0]) || phone[0] < '6' || phone[0] > '9')
        {
            return new ValidationResult("Mobile number must start with 6, 7, 8, or 9");
        }

        // Check if all characters are digits
        if (!phone.All(char.IsDigit))
        {
            return new ValidationResult("Mobile number must contain only digits");
        }

        // Final regex check
        if (Regex.IsMatch(phone, MobilePattern))
        {
            return ValidationResult.Success;
        }

        return new ValidationResult(
            ErrorMessage ?? "Please enter a valid 10-digit Indian mobile number starting with 6, 7, 8, or 9"
        );
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        MergeAttribute(context.Attributes, "data-val", "true");
        MergeAttribute(context.Attributes, "data-val-indianphone",
            ErrorMessage ?? "Please enter a valid 10-digit Indian mobile number");
    }

    private void MergeAttribute(IDictionary<string, string> attributes, string key, string value)
    {
        if (!attributes.ContainsKey(key))
        {
            attributes.Add(key, value);
        }
    }
}