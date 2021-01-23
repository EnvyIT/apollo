using System.Threading.Tasks;

namespace Apollo.Payment.Adapter
{
    public interface IPaymentWrapper<in TPayment, TResult>
    {
        Task<TResult> TransactionAsync(decimal amount, string description, TPayment paymentData);
    }
}
