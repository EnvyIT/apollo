using System;
using Apollo.Persistence.Attributes.Attributes;
using Apollo.Persistence.Attributes.Base;

namespace Apollo.Persistence.Test.Entity.Mock
{
    [EntityTable("city")]
    public class CityMock : BaseEntity<CityMock>
    {
        [EntityColumn("postal_code")]
        public string PostalCode { get; set; }

        [EntityColumn("name")]
        public string Name { get; set; }

        public override object Clone()
        {
            return MemberwiseClone();
        }

        public override bool Equals(CityMock other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && PostalCode == other.PostalCode && Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CityMock)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, PostalCode, Name);
        }
    }
}
