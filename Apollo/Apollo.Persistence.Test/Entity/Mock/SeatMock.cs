using System;
using Apollo.Persistence.Attributes.Attributes;
using Apollo.Persistence.Attributes.Base;

namespace Apollo.Persistence.Test.Entity.Mock
{
    [EntityTable("seat")]
    public class SeatMock : BaseEntity<SeatMock>
    {
        [EntityColumn("row_id", true)]
        public long RowId { get; set; }
        [EntityColumnRef("row_id")]
        public RowMock RowMock { get; set; }
        [EntityColumn("number")]
        public int Number { get; set; }
        [EntityColumn("locked")]
        public bool Locked { get; set; }
        [EntityColumn("layout_column")]
        public int LayoutColumn { get; set; }
        [EntityColumn("layout_row")]
        public int LayoutRow { get; set; }

        public override bool Equals(SeatMock other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return RowId == other.RowId && Number == other.Number && Locked == other.Locked && LayoutColumn == other.LayoutColumn && LayoutRow == other.LayoutRow && Id == other.Id;
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
            return Equals((SeatMock) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Number, LayoutColumn, LayoutRow, Locked, RowId);
        }
    }
}
