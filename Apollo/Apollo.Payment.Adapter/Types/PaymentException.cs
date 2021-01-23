using System;

namespace Apollo.Payment.Adapter.Types
{
    public class PaymentException : Exception
    {
        public PaymentError Error { get; }

        public PaymentException(PaymentError error, string message, Exception? innerException = null) :
            base(message, innerException)
        {
            Error = error;
        }
    }
}