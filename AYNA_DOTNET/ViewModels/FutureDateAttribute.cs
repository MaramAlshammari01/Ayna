using System.ComponentModel.DataAnnotations;

namespace Ayna.ViewModels.Validation
{
    /// <summary>
    /// Validates that a date is not in the past
    /// </summary>
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            if (value is DateTime dateTime)
            {
                if (dateTime < DateTime.Now)
                {
                    return new ValidationResult(
                        ErrorMessage ?? $"{validationContext.DisplayName} must be a future date."
                    );
                }
            }

            return ValidationResult.Success;
        }
    }
}
