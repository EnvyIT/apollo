using System;
using Apollo.Domain.Entity;
using FluentAssertions;

namespace Apollo.Domain.Test.Entity
{
    public class CinemaHallTest: EntityTest<CinemaHall>
    {
        private readonly string _attributeTableName = "cinema_hall";
        private readonly string _attributeColumnLabel = "label";
        private readonly string _attributeSizeRow = "size_row";
        private readonly string _attributeSizeColumn = "size_column";

        private readonly long _id = 1L;
        private readonly DateTime _rowVersion = DateTime.UtcNow;
        private readonly string _label = "Hall 1";
        private readonly int _sizeRow = 20;
        private readonly int _sizeColumn = 30;

        private readonly long _cloneId = 10L;
        private readonly DateTime _cloneRowVersion = DateTime.UtcNow.AddMinutes(1);
        private readonly string _cloneLabel = "Hall 15";
        private readonly int _cloneSizeRow = 200;
        private readonly int _cloneSizeColumn = 300;

        protected override void SetProperties(CinemaHall value)
        {
            value.Id = _id;
            value.RowVersion = _rowVersion;
            value.Label = _label;
            value.SizeRow = _sizeRow;
            value.SizeColumn = _sizeColumn;
        }

        protected override void SetCloneProperties(CinemaHall value)
        {
            value.Id = _cloneId;
            value.RowVersion = _cloneRowVersion;
            value.Label = _cloneLabel;
            value.SizeRow = _cloneSizeRow;
            value.SizeColumn = _cloneSizeColumn;
        }

        protected override void CheckProperties(CinemaHall value)
        {
            value.Id.Should().Be(_id);
            value.RowVersion.Should().Be(_rowVersion);
            value.Label.Should().Be(_label);
            value.SizeRow.Should().Be(_sizeRow);
            value.SizeColumn.Should().Be(_sizeColumn);
        }

        protected override void CheckClonedProperties(CinemaHall value)
        {
            value.Id.Should().Be(_cloneId);
            value.RowVersion.Should().Be(_cloneRowVersion);
            value.Label.Should().Be(_cloneLabel);
            value.SizeRow.Should().Be(_cloneSizeRow);
            value.SizeColumn.Should().Be(_cloneSizeColumn);
        }

        protected override int CalculateHashCode()
        {
            return HashCode.Combine(_id, _label, _sizeRow, _sizeColumn);
        }

        protected override string GetAttributeTableName()
        {
            return _attributeTableName;
        }

        protected override void CheckAttributeColumns()
        {
            Attribute_Column_Name_Should(_ => _.Label, _attributeColumnLabel);
            Attribute_Column_Name_Should(_ => _.SizeRow, _attributeSizeRow);
            Attribute_Column_Name_Should(_ => _.SizeColumn, _attributeSizeColumn);
        }
    }
}