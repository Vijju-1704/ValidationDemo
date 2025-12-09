using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

public class CustomEmailAttribute : ValidationAttribute, IClientModelValidator
{
    private const string EmailPattern = @"^[a-zA-Z0-9]([a-zA-Z0-9._-]*[a-zA-Z0-9])?@[a-zA-Z0-9]([a-zA-Z0-9-]*[a-zA-Z0-9])?(\.[a-zA-Z0-9]([a-zA-Z0-9-]*[a-zA-Z0-9])?)+$";

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return ValidationResult.Success;
        }

        string? email = value?.ToString()?.Trim();
        if (string.IsNullOrEmpty(email))
        {
            return ValidationResult.Success;
        }

        // Check length
        if (email.Length > 254)
        {
            return new ValidationResult("Email address is too long (maximum 254 characters)");
        }

        // Check for spaces
        if (email.Contains(" "))
        {
            return new ValidationResult("Email address cannot contain spaces");
        }

        // Check if starts with . or @
        if (email.StartsWith(".") || email.StartsWith("@"))
        {
            return new ValidationResult("Email cannot start with . or @");
        }

        // Check if ends with . or @
        if (email.EndsWith(".") || email.EndsWith("@"))
        {
            return new ValidationResult("Email cannot end with . or @");
        }

        // Check for consecutive dots
        if (email.Contains(".."))
        {
            return new ValidationResult("Email cannot contain consecutive dots");
        }

        // Check if contains @
        if (!email.Contains("@"))
        {
            return new ValidationResult("Email address must contain @ symbol");
        }

        // Check for multiple @
        if (email.Count(c => c == '@') > 1)
        {
            return new ValidationResult("Email address cannot contain multiple @ symbols");
        }

        // Split and validate parts
        var parts = email.Split('@');
        if (parts[0].Length == 0)
        {
            return new ValidationResult("Email must have characters before @");
        }

        if (parts[0].StartsWith(".") || parts[0].EndsWith("."))
        {
            return new ValidationResult("Local part cannot start or end with dot");
        }

        if (parts.Length < 2 || parts[1].Length == 0)
        {
            return new ValidationResult("Email must have domain after @");
        }

        if (!parts[1].Contains("."))
        {
            return new ValidationResult("Email must include domain extension (e.g., @gmail.com)");
        }

        if (parts[1].StartsWith(".") || parts[1].EndsWith("."))
        {
            return new ValidationResult("Domain cannot start or end with dot");
        }

        // Regex validation
        if (Regex.IsMatch(email, EmailPattern, RegexOptions.IgnoreCase))
        {
            return ValidationResult.Success;
        }

        return new ValidationResult(
            ErrorMessage ?? "Please enter a valid email address"
        );
    }

    public void AddValidation(ClientModelValidationContext context)
    {
        MergeAttribute(context.Attributes, "data-val", "true");
        MergeAttribute(context.Attributes, "data-val-customemail",
            ErrorMessage ?? "Please enter a valid email address");
    }

    private void MergeAttribute(IDictionary<string, string> attributes, string key, string value)
    {
        if (!attributes.ContainsKey(key))
        {
            attributes.Add(key, value);
        }
    }
}