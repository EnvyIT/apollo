using System;
using System.ComponentModel.DataAnnotations;
using Apollo.Util;

namespace Apollo.Core.Dto.ValidationAttributes
{
    public class MatchIdAttribute : ValidationAttribute
    {
        private readonly string _propertyName;

        public MatchIdAttribute(string propertyName)
        {
            _propertyName = propertyName ?? throw new ArgumentNullException(nameof(propertyName));
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var id = ReflectionUtils.GetPropertyValue<long>(validationContext.ObjectInstance, _propertyName);
            var dto = (BaseDto)value;
            if (dto == null || dto.Id == id)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage ?? $"Ids do not match");
        }

        
    }
}
