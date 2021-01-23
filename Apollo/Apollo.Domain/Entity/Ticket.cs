using System;
using Apollo.Persistence.Attributes.Attributes;
using Apollo.Persistence.Attributes.Base;

namespace Apollo.Domain.Entity
{
    [EntityTable("ticket")]
    public class Ticket : BaseEntity<Ticket>
    {
        [EntityColumn("printed")] 
        public DateTime Printed { get; set; }

        public override object Clone()
        {
            return MemberwiseClone();
        }

        public override bool Equals(Ticket other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && Printed == other.Printed;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Ticket)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Printed);
        }
    }
}
