namespace Apollo.Core.Exception
{
    public class PersistenceException : System.Exception
    {
        public PersistenceException()
        {
        }

        public PersistenceException(string message)
            : this(message, null)
        {
        }

        public PersistenceException(string message, System.Exception inner)
            : base(message, inner)
        {
        }
    }
}