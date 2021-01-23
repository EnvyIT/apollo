using System;
using Apollo.Persistence.Attributes.Attributes;
using Apollo.Persistence.Attributes.Base;

namespace Apollo.Domain.Entity
{
    [EntityTable("cinema_hall")]
    public class CinemaHall : BaseEntity<CinemaHall>
    {
        [EntityColumn("label")]
        public string Label { get; set; }

        [EntityColumn("size_row")]
        public int SizeRow { get; set; }

        [EntityColumn("size_column")]
        public int SizeColumn { get; set; }

        public override bool Equals(CinemaHall other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && Label == other.Label && SizeRow == other.SizeRow && SizeColumn == other.SizeColumn;
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
            return Equals((CinemaHall) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Label, SizeRow, SizeColumn);
        }
    }
}
