using System;
using System.ComponentModel.DataAnnotations;

namespace Apollo.Core.Dto.ValidationAttributes
{
    public class FutureAttribute : ValidationAttribute
    {
        private readonly double _hours;

        public FutureAttribute(double hours)
        {
            if (hours <= 0) throw new ArgumentOutOfRangeException(nameof(hours));
            _hours = hours;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime dateTime && dateTime >= DateTime.UtcNow.AddHours(_hours))
            { 
                return ValidationResult.Success;
            }
            return new ValidationResult(ErrorMessage ?? $"Current DateTime is is too close to DateTime.Now");
        }


    }
}
