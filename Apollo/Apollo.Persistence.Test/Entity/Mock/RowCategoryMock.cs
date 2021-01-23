using System;
using Apollo.Persistence.Attributes.Attributes;
using Apollo.Persistence.Attributes.Base;

namespace Apollo.Persistence.Test.Entity.Mock
{
    [EntityTable("row_category")]
    public class RowCategoryMock: BaseEntity<RowCategoryMock>
    {
        [EntityColumn("name")]
        public string Name { get; set; }
        [EntityColumn("price_factor")]
        public double PriceFactor { get; set; }

        public override bool Equals(RowCategoryMock other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name && PriceFactor.Equals(other.PriceFactor) && Id == other.Id;
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
            return Equals((RowCategoryMock) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, PriceFactor);
        }
    }
}
