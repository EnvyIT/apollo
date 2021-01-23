using System.Threading.Tasks;
using Apollo.Payment.Domain;

namespace Apollo.Payment
{
    public interface IPaymentApi<in TMethod> where TMethod: IPaymentMethod
    {
        Task TransactionAsync(decimal amount, string description, TMethod method);
    }
}