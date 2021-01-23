namespace Apollo.Payment.Adapter.Types
{
    public enum PaymentError
    {
        Unknown = 1,
        System = 2,
        ConnectionLost = 10 ,
        DataIncomplete = 30,
        DataInvalid = 31,
        CardExpired = 41 ,
        CardReportLost = 42,
        CardInsufficientFunds = 43,
        CardValidation = 44,
        CardInvalidName = 45,
        CardInvalid = 46
    }
}