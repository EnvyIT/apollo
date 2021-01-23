using System;
using System.Threading.Tasks;
using Apollo.Payment.Adapter.Types;
using Apollo.Payment.Domain;
using Apollo.Util.Logger;
using FHPay;

namespace Apollo.Payment.Adapter
{
    public class FhPayAdapter : BasePaymentAdapter<CreditCard, IPaymentMethod>
    {
        private static readonly IApolloLogger<FhPayAdapter> Logger = LoggerFactory.CreateLogger<FhPayAdapter>();

        private readonly IPaymentWrapper<CreditCard, PaymentResult> _paymentApi;

        public FhPayAdapter(IPaymentWrapper<CreditCard, PaymentResult> paymentApi)
        {
            _paymentApi = paymentApi ?? throw new ArgumentNullException(nameof(paymentApi));
        }

        protected override async Task CallTransaction(decimal amount, string description, CreditCard data)
        {
            try
            {
                ValidateResult(await _paymentApi.TransactionAsync(amount, description, data));
            }
            catch (ArgumentNullException argumentException)
            {
                var exception = new PaymentException(PaymentError.DataIncomplete, "Given data incomplete", argumentException);
                Logger.Here().Error(exception, "{CreditCard} data was incomplete", nameof(ApolloCreditCard));
                throw exception;
            }
            catch (ArgumentException argumentException)
            {
                var exception =  new PaymentException(PaymentError.DataInvalid, "Given data not valid", argumentException);
                Logger.Here().Error(exception, "{CreditCard} data was invalid", nameof(ApolloCreditCard));
                throw exception;
            }
            catch (NetworkConnectionException networkConnectionException)
            {
                var exception = new PaymentException(PaymentError.ConnectionLost, "Connection lost", networkConnectionException);
                Logger.Here().Error(exception, "Connection was lost during transaction");
                throw exception;
            }
        }

        protected override CreditCard ConvertPayment(IPaymentMethod payment)
        {
            if (!(payment is ApolloCreditCard creditCard))
            {
                var paymentException = new PaymentException(PaymentError.DataIncomplete, "Card was null");
                Logger.Here().Error(paymentException, "{CreditCard} was null", nameof(ApolloCreditCard));
                throw paymentException;
            }

            try
            {
                return new CreditCard(
                    creditCard.OwnerName,
                    new CreditCardNumber(creditCard.CardNumber),
                    new ExpirationDate(creditCard.Expiration.Month, creditCard.Expiration.Year),
                    new CardValidationCode(creditCard.Cvc)
                );
            }
            catch (Exception exception)
            {
                var paymentException = new PaymentException(PaymentError.CardInvalid, "Invalid card", exception);
                Logger.Here().Error(paymentException, "{CreditCard} was invalid", nameof(ApolloCreditCard));
                throw paymentException;
            }
        }

        private static void ValidateResult(PaymentResult result)
        {
            switch (result)
            {
                case PaymentResult.PaymentSuccessful:
                    return;
                case PaymentResult.CardExpired:
                    throw new PaymentException(PaymentError.CardExpired, "Card expired");
                case PaymentResult.CardReportedLost:
                    throw new PaymentException(PaymentError.CardReportLost, "Card reported as lost");
                case PaymentResult.InsufficientFunds:
                    throw new PaymentException(PaymentError.CardInsufficientFunds, "Insufficient funds");
                case PaymentResult.InvalidCardValidationCode:
                    throw new PaymentException(PaymentError.CardValidation, "Card validation failed");
                case PaymentResult.InvalidName:
                    throw new PaymentException(PaymentError.CardInvalidName, "Invalid name");
                default:
                    throw new PaymentException(PaymentError.Unknown, "Unknown failure");
            }
        }
    }
}
