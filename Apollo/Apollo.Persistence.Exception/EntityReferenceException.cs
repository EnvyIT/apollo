namespace Apollo.Persistence.Exception
{
    public class EntityReferenceException : System.Exception
    {
        public EntityReferenceException()
        {
        }

        public EntityReferenceException(long referenceId)
            : this(referenceId, "")
        {
        }

        public EntityReferenceException(long referenceId, string message)
            : this(referenceId, message, null)
        {
        }

        public EntityReferenceException(long referenceId, string message, System.Exception inner)
            : base($"{referenceId} is referenced. {message}", inner)
        {
        }
    }
}