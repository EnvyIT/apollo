using System;
using Apollo.Persistence.Attributes.Attributes;
using Apollo.Persistence.Attributes.Base;

namespace Apollo.Domain.Entity
{
    [EntityTable("actor")]
    public class Actor : BaseEntity<Actor>
    {
        [EntityColumn("first_name")]
        public string FirstName { get; set; }
        [EntityColumn("last_name")]
        public string LastName { get; set; }
        
        public string Name => string.Empty == LastName ? FirstName : $"{FirstName} {LastName}";
        
        public override bool Equals(Actor other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && FirstName == other.FirstName && LastName == other.LastName;
        }

        public override object Clone()
        {
            return MemberwiseClone();
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Actor) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, FirstName, LastName);
        }
    }
}
