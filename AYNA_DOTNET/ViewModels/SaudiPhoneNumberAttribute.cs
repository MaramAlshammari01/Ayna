using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Ayna.ViewModels.Validation
{
    /// <summary>
    /// Validates Saudi phone numbers (+9665xxxxxxxx or 05xxxxxxxx)
    /// </summary>
    public class SaudiPhoneNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return ValidationResult.Success;

            string phoneNumber = value.ToString();
            string pattern = @"^((\+9665\d{8})|(05\d{8}))$";

            if (Regex.IsMatch(phoneNumber, pattern))
                return ValidationResult.Success;

            return new ValidationResult(
                ErrorMessage ?? $"{validationContext.DisplayName} must be a valid phone number. Example: +966566193395 or 0566193395"
            );
        }
    }
}
