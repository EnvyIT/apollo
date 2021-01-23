using System;
using Apollo.Persistence.Attributes.Attributes;
using Apollo.Persistence.Attributes.Base;

namespace Apollo.Domain.Entity
{
    [EntityTable("row_category")]
    public class RowCategory: BaseEntity<RowCategory>
    {
        [EntityColumn("name")]
        public string Name { get; set; }
        [EntityColumn("price_factor")]
        public double PriceFactor { get; set; }

        public override bool Equals(RowCategory other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && Name == other.Name && PriceFactor.Equals(other.PriceFactor);
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
            return Equals((RowCategory) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, PriceFactor);
        }
    }
}
