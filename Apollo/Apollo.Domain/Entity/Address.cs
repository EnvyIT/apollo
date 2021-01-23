using System;
using Apollo.Persistence.Attributes.Attributes;
using Apollo.Persistence.Attributes.Base;

namespace Apollo.Domain.Entity
{
    [EntityTable("address")]
    public class Address : BaseEntity<Address>
    {
        [EntityColumn("street")]
        public string Street { get; set; }

        [EntityColumn("number")]
        public int Number { get; set; }

        [EntityColumn("city_id")]
        public long CityId { get; set; }

        [EntityColumnRef("city")]
        public City City { get; set; }

        public override object Clone()
        {
            var clone = (Address)MemberwiseClone();
            clone.City = (City)City?.Clone();
            return clone;
        }

        public override bool Equals(Address other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && Street == other.Street && Number == other.Number && CityId == other.CityId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Address)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Street, Number, CityId);
        }
    }
}