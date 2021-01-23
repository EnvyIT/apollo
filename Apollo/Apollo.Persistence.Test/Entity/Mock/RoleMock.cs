using System;
using Apollo.Persistence.Attributes.Attributes;
using Apollo.Persistence.Attributes.Base;

namespace Apollo.Persistence.Test.Entity.Mock
{
    [EntityTable("role")]
    public class RoleMock : BaseEntity<RoleMock>
    {
        [EntityColumn("label")]
        public string Label { get; set; }

        [EntityColumn("max_reservations")]
        public int MaxReservations { get; set; }

        public override object Clone()
        {
            return MemberwiseClone();
        }

        public override bool Equals(RoleMock other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && Label == other.Label && MaxReservations == other.MaxReservations;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RoleMock)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Label, MaxReservations);
        }
    }
}
