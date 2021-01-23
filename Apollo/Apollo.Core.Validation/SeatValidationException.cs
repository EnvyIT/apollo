namespace Apollo.Core.Validation
{
    public class SeatValidationException : System.Exception
    {
        public SeatValidationException()
        {
        }

        public SeatValidationException(string message)
            : this(message, null)
        {
        }

        public SeatValidationException(string message, System.Exception inner)
            : base(message, inner)
        {
        }
    }
}
