using System;
using System.Reflection;

namespace Apollo.Persistence.FluentEntity.Types
{
    public class FluentEntityProperty<T>
    {
        public T Attribute { get; private set; }

        public PropertyInfo Property { get; private set; }

        public FluentEntityProperty(T attribute, PropertyInfo property)
        {
            Attribute = attribute;
            Property = property ?? throw new ArgumentNullException(nameof(property));
        }
    }
}
