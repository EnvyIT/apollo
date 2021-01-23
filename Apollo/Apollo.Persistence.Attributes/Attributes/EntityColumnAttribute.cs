namespace Apollo.Persistence.Attributes.Attributes
{
    public class EntityColumnAttribute : System.Attribute
    {
        public string Name { get; private set; }

        public bool IsKey { get; private set; }


        public EntityColumnAttribute(string name, bool isKey = false)
        {
            Name = name;
            IsKey = isKey;
        }
    }
}
