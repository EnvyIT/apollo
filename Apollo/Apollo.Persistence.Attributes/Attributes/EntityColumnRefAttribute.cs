using System;

namespace Apollo.Persistence.Attributes.Attributes
{
    public class EntityColumnRefAttribute : Attribute
    {
        public string Name { get; private set; }

        public EntityColumnRefAttribute(string name)
        {
            Name = name;
        }
    }
}
