using System.ComponentModel.DataAnnotations;

namespace ReadIraq.Authorization.Users
{
    public class NoWhiteSpace : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var propertyName = validationContext.DisplayName;
            var stringValue = value as string;

            if (stringValue != null && stringValue.Contains(' '))
            {
                return new ValidationResult(string.Format(ErrorMessageString, propertyName));
            }

            return ValidationResult.Success;
        }
    }
}
