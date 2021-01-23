using System;
using Apollo.Persistence.Attributes.Attributes;
using Apollo.Persistence.Attributes.Contracts;

namespace Apollo.Persistence.Attributes.Base
{
    public abstract class BaseEntity<T> : IConcurrentEntity, ICloneable, IIdentifiable<T>, ISoftDeleteable
    {
        [EntityColumn("row_version")]
        public DateTime RowVersion { get; set; }

        [EntityColumn("id", true)]
        public long Id { get; set; }

        [EntityColumn("deleted")]
        public bool Deleted { get; set; }

        public abstract object Clone();
        public abstract bool Equals(T other);
    }
}
