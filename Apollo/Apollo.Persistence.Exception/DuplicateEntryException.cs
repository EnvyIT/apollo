namespace Apollo.Persistence.Exception
{
    public class DuplicateEntryException : System.Exception
    {
        public DuplicateEntryException()
        {
        }

        public DuplicateEntryException(long id)
            : this(id, "")
        {
        }

        public DuplicateEntryException(long id, string message)
            : this(id, message, null)
        {
        }

        public DuplicateEntryException(long id, string message, System.Exception inner)
            : base($"{id} already exist. {message}", inner)
        {
        }
    }
}