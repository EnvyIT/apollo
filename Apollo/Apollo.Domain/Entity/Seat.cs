using System;
using Apollo.Persistence.Attributes.Attributes;
using Apollo.Persistence.Attributes.Base;

namespace Apollo.Domain.Entity
{
    [EntityTable("seat")]
    public class Seat : BaseEntity<Seat>
    {
        [EntityColumn("row_id")]
        public long RowId { get; set; }
        [EntityColumnRef("row")]
        public Row Row { get; set; }
        [EntityColumn("number")]
        public int Number { get; set; }
        [EntityColumn("locked")]
        public bool Locked { get; set; }
        [EntityColumn("layout_column")]
        public int LayoutColumn { get; set; }
        [EntityColumn("layout_row")]
        public int LayoutRow { get; set; }

        public override bool Equals(Seat other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && RowId == other.RowId && Number == other.Number && Locked == other.Locked && LayoutColumn == other.LayoutColumn && LayoutRow == other.LayoutRow;
        }

        public override object Clone()
        {
            var seat = (Seat)MemberwiseClone();
            seat.Row = (Row) seat.Row?.Clone();
            return seat;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Seat) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, RowId, Number, Locked, LayoutColumn, LayoutRow);
        }
    }
}
