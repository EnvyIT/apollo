using System;
using Apollo.Persistence.Attributes.Attributes;
using Apollo.Persistence.Attributes.Base;

namespace Apollo.Persistence.Test.Entity.Mock
{
    [EntityTable("row")]
    public class RowMock : BaseEntity<RowMock>
    {
        [EntityColumn("number")]
        public int Number { get; set; }
        [EntityColumn("category")]
        public long Category { get; set; }
        [EntityColumn("cinema_hall")]
        public long CinemaHallId { get; set; }
        [EntityColumnRef("cinema_hall")]
        public CinemaHallMock CinemaHallMock { get; set; }

        public override bool Equals(RowMock other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Number == other.Number && Category == other.Category && CinemaHallMock == other.CinemaHallMock && Id == other.Id;
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
            return Equals((RowMock) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Number, Category, CinemaHallId);
        }
    }
}
