using System.ComponentModel.DataAnnotations;

namespace Ayna.ViewModels.Validation
{
    /// <summary>
    /// Validates Saudi commercial registration number
    /// </summary>
    public class CommercialRegistrationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return ValidationResult.Success;

            string crNumber = value.ToString();

            // CR number should be 10 digits
            if (crNumber.Length != 10 || !crNumber.All(char.IsDigit))
            {
                return new ValidationResult(
                    ErrorMessage ?? $"{validationContext.DisplayName} must be exactly 10 digits."
                );
            }

            return ValidationResult.Success;
        }
    }
}
