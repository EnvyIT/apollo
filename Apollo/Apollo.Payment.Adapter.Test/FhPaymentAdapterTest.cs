using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Apollo.Payment.Adapter.Test.Mock;
using Apollo.Payment.Adapter.Types;
using Apollo.Payment.Domain;
using FHPay;
using FluentAssertions;
using NUnit.Framework;

namespace Apollo.Payment.Adapter.Test
{
    public class FhPaymentAdapterTest
    {
        private FhPaymentApiMock _paymentMock;
        private IPaymentApi<ApolloCreditCard> _paymentApi;
        private const string ApiKey = "12341234-5555-125-1235-09877123";
        private const string ValidCardNumber = "4005550000000019";
        private const string InvalidCardNumber = "4005550000000010";
        private const string Cvc = "111";
        private static readonly DateTime ExpirationDate = new DateTime(2023, 4, 1);

        private readonly Dictionary<PaymentResult, (PaymentError Error, string ExceptionMessage)> _possibleResults =
            new Dictionary<PaymentResult, (PaymentError Error, string ExceptionMessage)>
            {
                {PaymentResult.CardExpired, (PaymentError.CardExpired, "Card expired")},
                {PaymentResult.CardReportedLost, (PaymentError.CardReportLost, "Card reported as lost")},
                {PaymentResult.InsufficientFunds, (PaymentError.CardInsufficientFunds, "Insufficient funds")},
                {PaymentResult.InvalidCardValidationCode, (PaymentError.CardValidation, "Card validation failed")},
                {PaymentResult.InvalidName, (PaymentError.CardInvalidName, "Invalid name")}
            };

        [SetUp]
        public void Setup()
        {
            _paymentMock = new FhPaymentApiMock(ApiKey);
            _paymentApi = new FhPayAdapter(_paymentMock);
        }

        [Test]
        public void WrongApiKey_ShouldThrowArgumentException()
        {
            Action createPaymentApi = () =>
                _paymentApi = new FhPayAdapter(new FhPaymentApiMock("1234-1235-1235-151351-12352135"));
            createPaymentApi.Should().ThrowExactly<ArgumentException>();
        }

        [Test]
        public void NoApi_ShouldThrowArgumentNullException()
        {
            Action createPaymentApi = () => _paymentApi = new FhPayAdapter(null);
            createPaymentApi.Should().ThrowExactly<ArgumentNullException>();
        }

        [Test]
        public async Task CreditCardNull_ShouldThrowArgumentNullException()
        {
            Func<Task> transaction = async () => await _paymentApi.TransactionAsync(15M, "Amazon order", null);
            var ex = await transaction.Should().ThrowExactlyAsync<PaymentException>();
            ex.WithMessage("Card was null");
            ex.And.Error.Should().Be(PaymentError.DataIncomplete);
        }

        [Test]
        public async Task DescriptionNull_ShouldThrowArgumentNullException()
        {
            var creditCard = new ApolloCreditCard
            {
                CardNumber = ValidCardNumber,
                Cvc = Cvc,
                Expiration = ExpirationDate,
                OwnerBirthday = new DateTime(1988, 3, 15),
                OwnerName = "Hubert von Goisern"
            };
            Func<Task> transaction = async () => await _paymentApi.TransactionAsync(15M, null, creditCard);
            var ex = await transaction.Should().ThrowExactlyAsync<PaymentException>();
            ex.WithMessage("Given data incomplete");
            ex.And.Error.Should().Be(PaymentError.DataIncomplete);
        }

        [Test]
        public async Task DescriptionTooLong_ShouldThrowArgumentException()
        {
            var creditCard = new ApolloCreditCard
            {
                CardNumber = ValidCardNumber,
                Cvc = Cvc,
                Expiration = ExpirationDate,
                OwnerBirthday = new DateTime(1988, 3, 15),
                OwnerName = "Hubert von Goisern"
            };
            const string longDescription =
                "OOyGGKBzNYeWvctIGZJxMOMcBrXwvhxNOKIGOrlQokgQCuskikyCADUTqLqgjgHbTnaQSkOKlTaFkXtHizPBBsjzCtVkzeanZigmM";
            Func<Task> transaction = async () => await _paymentApi.TransactionAsync(15M, longDescription, creditCard);
            var ex = await transaction.Should().ThrowExactlyAsync<PaymentException>();
            ex.WithMessage("Given data not valid");
            ex.And.Error.Should().Be(PaymentError.DataInvalid);
        }

        [Test]
        public async Task AmountNegative_ShouldThrowArgumentOutOfRangeException()
        {
            var creditCard = new ApolloCreditCard
            {
                CardNumber = ValidCardNumber,
                Cvc = Cvc,
                Expiration = ExpirationDate,
                OwnerBirthday = new DateTime(1988, 3, 15),
                OwnerName = "Hubert von Goisern"
            };
            Func<Task> transaction = async () => await _paymentApi.TransactionAsync(-15M, "Amazon payment", creditCard);
            var ex = await transaction.Should().ThrowExactlyAsync<PaymentException>();
            ex.WithMessage("Given data not valid");
            ex.And.Error.Should().Be(PaymentError.DataInvalid);
        }

        [Test]
        public async Task AmountExceeds10k_ShouldThrowArgumentOutOfRangeException()
        {
            var creditCard = new ApolloCreditCard
            {
                CardNumber = ValidCardNumber,
                Cvc = Cvc,
                Expiration = DateTime.UtcNow.AddYears(2),
                OwnerBirthday = new DateTime(1988, 3, 15),
                OwnerName = "Hubert von Goisern"
            };
            Func<Task> transaction = async () =>
                await _paymentApi.TransactionAsync(10_000.01M, "Amazon payment", creditCard);
            var ex = await transaction.Should().ThrowExactlyAsync<PaymentException>();
            ex.WithMessage("Given data not valid");
            ex.And.Error.Should().Be(PaymentError.DataInvalid);
        }

        [Test]
        public async Task NoConnection_ShouldThrow_ConnectionLost()
        {
            _paymentMock.ConnectionDisabled = true;

            var creditCard = new ApolloCreditCard
            {
                CardNumber = ValidCardNumber,
                Cvc = Cvc,
                Expiration = DateTime.UtcNow.AddYears(2),
                OwnerBirthday = new DateTime(1988, 3, 15),
                OwnerName = "Hubert von Goisern"
            };
            Func<Task> transaction = async () =>
                await _paymentApi.TransactionAsync(100.0M, "Amazon payment", creditCard);
            var ex = await transaction.Should().ThrowExactlyAsync<PaymentException>();
            ex.WithMessage("Connection lost");
            ex.And.Error.Should().Be(PaymentError.ConnectionLost);
        }

        [Test]
        public async Task InvalidState_ShouldThrow_UnknownError()
        {
            _paymentMock.InvalidState = true;

            var creditCard = new ApolloCreditCard
            {
                CardNumber = ValidCardNumber,
                Cvc = Cvc,
                Expiration = DateTime.UtcNow.AddYears(2),
                OwnerBirthday = new DateTime(1988, 3, 15),
                OwnerName = "Hubert von Goisern"
            };
            Func<Task> transaction = async () =>
                await _paymentApi.TransactionAsync(100.0M, "Amazon payment", creditCard);
            var ex = await transaction.Should().ThrowExactlyAsync<PaymentException>();
            ex.WithMessage("Unknown error");
            ex.And.Error.Should().Be(PaymentError.Unknown);
        }

        [Test]
        public async Task ValidState_Should_NotThrow()
        {
            _paymentMock.CurrentResult = PaymentResult.PaymentSuccessful;

            var creditCard = new ApolloCreditCard
            {
                CardNumber = ValidCardNumber,
                Cvc = Cvc,
                Expiration = DateTime.UtcNow.AddYears(2),
                OwnerBirthday = new DateTime(1988, 3, 15),
                OwnerName = "Hubert von Goisern"
            };
            Func<Task> transaction = async () =>
                await _paymentApi.TransactionAsync(100.0M, "Amazon payment", creditCard);
            await transaction.Should().NotThrowAsync();
        }

        [Test]
        public async Task InvalidCard_Should_Throw()
        {
            var creditCard = new ApolloCreditCard
            {
                CardNumber = ValidCardNumber,
                Cvc = Cvc,
                OwnerBirthday = new DateTime(1988, 3, 15),
                OwnerName = "Hubert von Goisern"
            };
            Func<Task> transaction = async () =>
                await _paymentApi.TransactionAsync(100.0M, "Amazon payment", creditCard);
            var ex = await transaction.Should().ThrowExactlyAsync<PaymentException>();
            ex.WithMessage("Invalid card");
            ex.And.Error.Should().Be(PaymentError.CardInvalid);
        }

        [Test]
        public async Task Test_Different_ErrorResults()
        {
            var creditCard = new ApolloCreditCard
            {
                CardNumber = ValidCardNumber,
                Cvc = Cvc,
                Expiration = DateTime.UtcNow.AddYears(2),
                OwnerBirthday = new DateTime(1988, 3, 15),
                OwnerName = "Hubert von Goisern"
            };
            Func<Task> transaction = async () =>
                await _paymentApi.TransactionAsync(100.0M, "Amazon payment", creditCard);

            foreach (var result in _possibleResults)
            {
                _paymentMock.CurrentResult = result.Key;
                var ex = await transaction.Should().ThrowExactlyAsync<PaymentException>();
                ex.WithMessage(result.Value.ExceptionMessage);
                ex.And.Error.Should().Be(result.Value.Error);
            }
        }


        [Test]
        public async Task InvalidCardNumber_ShouldThrowPaymentException()
        {
            var creditCard = new ApolloCreditCard
            {
                CardNumber = InvalidCardNumber,
                Cvc = Cvc,
                OwnerBirthday = new DateTime(1988, 3, 15),
                Expiration =  DateTime.UtcNow.AddMonths(1),
                OwnerName = "Hubert von Goisern"
            };
            Func<Task> transaction = async () =>
                await _paymentApi.TransactionAsync(100.0M, "Amazon payment", creditCard);
            var ex = await transaction.Should().ThrowExactlyAsync<PaymentException>();
            ex.WithMessage("Invalid card");
            ex.And.Error.Should().Be(PaymentError.CardInvalid);
        }
    }
}