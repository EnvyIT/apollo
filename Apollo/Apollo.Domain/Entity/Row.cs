using System;
using Apollo.Persistence.Attributes.Attributes;
using Apollo.Persistence.Attributes.Base;

namespace Apollo.Domain.Entity
{
    [EntityTable("row")]
    public class Row : BaseEntity<Row>
    {
        [EntityColumn("number")]
        public int Number { get; set; }

        [EntityColumn("category")]
        public long CategoryId { get; set; }

        [EntityColumnRef("row_category")]
        public RowCategory Category { get; set; }

        [EntityColumn("cinema_hall")]
        public long CinemaHallId { get; set; }

        [EntityColumnRef("cinema_hall")]
        public CinemaHall CinemaHall { get; set; }

        public override bool Equals(Row other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && Number == other.Number && CategoryId == other.CategoryId && CinemaHallId == other.CinemaHallId;
        }

        public override object Clone()
        {
            var row = (Row) MemberwiseClone();
            row.Category = (RowCategory) row.Category?.Clone();
            row.CinemaHall = (CinemaHall) row.CinemaHall?.Clone();
            return row;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Row) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Number, CinemaHallId, CategoryId);
        }
    }
}
