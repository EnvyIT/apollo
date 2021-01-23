using System;

namespace Apollo.Payment.Domain
{
    public class ApolloCreditCard: IPaymentMethod
    {
        public string CardNumber { get; set; }
        public string OwnerName { get; set; }
        public DateTime OwnerBirthday { get; set; }
        public DateTime Expiration { get; set; }
        public string Cvc { get; set; }
    }
}