namespace Apollo.Persistence.Attributes.Attributes
{
    public class EntityTableAttribute : System.Attribute
    {
        public string Source { get; private set; }

        public EntityTableAttribute(string source)
        {
            Source = source;
        }
    }
}
