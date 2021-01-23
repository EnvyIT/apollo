using Apollo.Core.Types;
using Apollo.Payment;
using Apollo.Payment.Domain;
using Microsoft.Extensions.Configuration;

namespace Apollo.Core.Interfaces
{
    public interface IPaymentFactory
    {
        IConfigurationRoot ConfigurationRoot { get; set; }
        IPaymentApi<IPaymentMethod> CreatePayment(PaymentType paymentType);
    }
}