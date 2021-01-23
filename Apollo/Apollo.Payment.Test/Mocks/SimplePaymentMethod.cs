using Apollo.Payment.Domain;

namespace Apollo.Payment.Test.Mocks
{
    public class SimplePaymentMethod: IPaymentMethod
    {
        public string Id { get; set; }
    }
}
