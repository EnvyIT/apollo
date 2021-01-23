using System;
using System.Text.RegularExpressions;

namespace Apollo.Util
{
    public static class CreditCardValidatorHelper
    {
        private static readonly Regex RegexCreditCardNumber = new Regex(
            @"^(?:4[0-9]{12}(?:[0-9]{3})?|(?:5[1-5][0-9]{2}|222[1-9]|22[3-9][0-9]|2[3-6][0-9]{2}|27[01][0-9]|2720)[0-9]{12}|3[47][0-9]{13}|3(?:0[0-5]|[68][0-9])[0-9]{11}|6(?:011|5[0-9]{2})[0-9]{12}|(?:2131|1800|35\d{3})\d{11})$");

        private static readonly Regex RegexCvc = new Regex(@"^[0-9]{3,4}$");
        private static readonly int MinNameLength = 3;
        private static readonly int MaxCardYears = 6;

        public static bool IsValidCardNumber(string value)
        {
            return ValidateRegex(RegexCreditCardNumber, value);
        }

        public static bool IsValidCvcNumber(string value)
        {
            return ValidateRegex(RegexCvc, value);
        }

        public static bool IsValidName(string value)
        {
            return value != null && value.Length >= MinNameLength;
        }

        public static bool IsValidExpirationDate(DateTime? value)
        {
            return value != null && value > DateTime.Now && value < DateTime.Now.AddYears(MaxCardYears);
        }

        private static bool ValidateRegex(Regex regex, string value)
        {
            return value != null && regex.IsMatch(value);
        }
    }
}