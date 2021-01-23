namespace Apollo.Persistence.Exception
{
    public class InvalidEntityIdException : System.Exception
    {
        public InvalidEntityIdException()
        {
        }

        public InvalidEntityIdException(long id)
            : this(id, "")
        {
        }

        public InvalidEntityIdException(long id, string message)
            : this(id, message, null)
        {
        }

        public InvalidEntityIdException(long id, string message, System.Exception inner)
            : base($"{id} not exist. {message}", inner)
        {
        }
    }
}