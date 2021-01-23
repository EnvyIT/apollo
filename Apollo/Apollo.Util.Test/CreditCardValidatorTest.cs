using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace Apollo.Util.Test
{
    public class CreditCardValidatorTest
    {
        private const string ValidCardNumber = "4005550000000019";
        private const string InvalidCardNumber = "400555000000000";
        private const string ValidCvc = "111";
        private const string AnotherValidCvc = "1114";
        private const string TooShortCvc = "32";
        private const string TooLongCvc = "32135";

        [Test] public void ValidateValidCardNumber_ShouldReturnTrue()
        {
            var validationResult = CreditCardValidatorHelper.IsValidCardNumber(ValidCardNumber);
            validationResult.Should().BeTrue();
        }

        [Test]
        public void ValidateInvalidCardNumber_ShouldReturnFalse()
        {
            var validationResult = CreditCardValidatorHelper.IsValidCardNumber(InvalidCardNumber);
            validationResult.Should().BeFalse();
        }

        [Test]
        public void ValidateCvcWith3Digits_ShouldReturnTrue()
        {
            var validationResult = CreditCardValidatorHelper.IsValidCvcNumber(ValidCvc);
            validationResult.Should().BeTrue();
        }

        [Test]
        public void ValidateCvcWith4Digits_ShouldReturnTrue()
        {
            var validationResult = CreditCardValidatorHelper.IsValidCvcNumber(AnotherValidCvc);
            validationResult.Should().BeTrue();
        }

        [Test]
        public void ValidateCvcTooShort_ShouldReturnFalse()
        {
            var validationResult = CreditCardValidatorHelper.IsValidCvcNumber(TooShortCvc);
            validationResult.Should().BeFalse();
        }

        [Test]
        public void ValidateCvcTooLong_ShouldReturnFalse()
        {
            var validationResult = CreditCardValidatorHelper.IsValidCvcNumber(TooLongCvc);
            validationResult.Should().BeFalse();
        }

        [Test]
        public void IsValidName_ShouldReturnTrue()
        {
            var validationResult = CreditCardValidatorHelper.IsValidName("Maximilian Mustermann");
            validationResult.Should().BeTrue();
        }

        [Test]
        public void IsValidNameWithNull_ShouldReturnFalse()
        {
            var validationResult = CreditCardValidatorHelper.IsValidName(null);
            validationResult.Should().BeFalse();
        }

        [Test]
        public void IsValidNameWithEmptyString_ShouldReturnFalse()
        {
            var validationResult = CreditCardValidatorHelper.IsValidName(string.Empty);
            validationResult.Should().BeFalse();
        }


        [Test]
        public void ExpirationDateTomorrow_ShouldReturnTrue()
        {
            var validationResult = CreditCardValidatorHelper.IsValidExpirationDate(DateTime.Today.AddDays(1));
            validationResult.Should().BeTrue();
        }

        [Test]
        public void ExpirationDateToday_ShouldReturnFalse()
        {
            var validationResult = CreditCardValidatorHelper.IsValidExpirationDate(DateTime.Today);
            validationResult.Should().BeFalse();
        }
    }
}
