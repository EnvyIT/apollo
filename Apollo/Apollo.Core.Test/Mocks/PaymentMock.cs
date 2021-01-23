using System.Threading.Tasks;
using Apollo.Payment;
using Apollo.Payment.Adapter.Types;
using Apollo.Payment.Domain;

namespace Apollo.Core.Test.Mocks
{
    public class PaymentMock : IPaymentApi<IPaymentMethod>
    {
        public bool IsInvalid { private get; set; } = false;

        public decimal Amount { get; private set; }

        public string Description { get; private set; }

        public IPaymentMethod PaymentMethod { get; private set; }

        public async Task TransactionAsync(decimal amount, string description, IPaymentMethod method)
        {
            if (IsInvalid)
            {
                throw new PaymentException(PaymentError.System, "Invalid");
            }

            await Task.Delay(100);

            Amount = amount;
            Description = description;
            PaymentMethod = method;
        }
    }
}