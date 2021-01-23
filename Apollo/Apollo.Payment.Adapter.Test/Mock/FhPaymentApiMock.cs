using System;
using System.Threading.Tasks;
using FHPay;

namespace Apollo.Payment.Adapter.Test.Mock
{
    public class FhPaymentApiMock : IPaymentWrapper<CreditCard, PaymentResult>
    {
        public PaymentResult CurrentResult { private get; set; } = PaymentResult.PaymentSuccessful;
        public bool ConnectionDisabled { private get; set; } = false;
        public bool InvalidState { private get; set; } = false;

        public FhPaymentApiMock(string apiKey)
        {
            if (apiKey == null || apiKey != "12341234-5555-125-1235-09877123")
            {
                throw new ArgumentException("Invalid key");
            }
        }

        public Task<PaymentResult> TransactionAsync(decimal amount, string description, CreditCard creditCard)
        {
            if (creditCard == null)
            {
                throw new ArgumentNullException(nameof(creditCard));
            }

            if (description == null)
            {
                throw new ArgumentNullException(nameof(description));
            }

            if (description.Length > 100)
            {
                throw new ArgumentException(nameof(description));
            }

            if (amount <= 0M || amount > 10000M)
            {
                throw new ArgumentOutOfRangeException(nameof(amount));
            }

            if (InvalidState)
            {
                throw new AggregateException();
            }

            if (ConnectionDisabled)
            {
                throw new NetworkConnectionException();
            }

            return Task.FromResult(CurrentResult);
        }
    }
}