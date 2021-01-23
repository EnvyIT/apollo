namespace Apollo.Terminal.Types.TransferObject
{
    public class ValidResultTransferObject<TValue>
    {
        public bool IsValid { get; }

        public TValue Value { get; }

        public ValidResultTransferObject(bool isValid)
        {
            IsValid = isValid;
        }

        public ValidResultTransferObject(bool isValid, TValue value)
        {
            IsValid = isValid;
            Value = value;
        }
    }
}
