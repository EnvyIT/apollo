using System.Threading.Tasks;
using FHPay;

namespace Apollo.Payment.Adapter.Wrapper
{
    public class FhPayWrapper : IPaymentWrapper<CreditCard, PaymentResult>
    {
        private readonly PaymentApi _api;

        public FhPayWrapper(PaymentApi api)
        {
            _api = api;
        }

        public async Task<PaymentResult> TransactionAsync(decimal amount, string description, CreditCard creditCard)
        {
            return await _api.CreateTransactionAsync(amount, creditCard, description);
        }
    }
}