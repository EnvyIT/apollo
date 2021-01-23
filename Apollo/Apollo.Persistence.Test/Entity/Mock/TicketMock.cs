using System;
using Apollo.Persistence.Attributes.Attributes;
using Apollo.Persistence.Attributes.Base;

namespace Apollo.Persistence.Test.Entity.Mock
{
    [EntityTable("ticket")]
    public class TicketMock : BaseEntity<TicketMock>
    {
        [EntityColumn("printed")]
        public DateTime Printed { get; set; }

        public override object Clone()
        {
            return MemberwiseClone();
        }

        public override bool Equals(TicketMock other)
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
            return Equals((TicketMock)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Printed);
        }
    }
}
