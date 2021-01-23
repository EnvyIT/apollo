using System.Threading.Tasks;
using Apollo.Payment.Adapter.Types;

namespace Apollo.Payment.Test.Mocks
{
    public class SimplePaymentMock : IPaymentApi<SimplePaymentMethod>
    {
        public async Task TransactionAsync(decimal amount, string description, SimplePaymentMethod method)
        {
            await Task.Delay(250);

            if (string.IsNullOrEmpty(method.Id))
            {
                throw new PaymentException(PaymentError.Unknown, "Invalid");
            }
        }
    }
}