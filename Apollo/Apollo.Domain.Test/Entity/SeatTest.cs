using System;
using Apollo.Domain.Entity;
using FluentAssertions;

namespace Apollo.Domain.Test.Entity
{
    public class SeatTest : EntityTest<Seat>
    {
        private readonly string _attributeTableName = "seat";
        private readonly string _attributeColumnRowId = "row_id";
        private readonly string _attributeColumnNumber = "number";
        private readonly string _attributeColumnLocked = "locked";
        private readonly string _attributeColumnLayoutColumn = "layout_column";
        private readonly string _attributeColumnLayoutRow = "layout_row";
        private readonly string _attributeColumnRefRow = "row";

        private readonly long _id = 1L;
        private readonly DateTime _rowVersion = DateTime.UtcNow;
        private readonly long _rowId = 10;
        private readonly int _number = 15;
        private readonly bool _locked = true;
        private readonly int _layoutColumn = 5;
        private readonly int _layoutRow = 6;
        private readonly Row _row = new Row { Id = 20, RowVersion = DateTime.UtcNow, CategoryId = 30, CinemaHallId = 40 };


        private readonly long _cloneId = 12L;
        private readonly DateTime _cloneRowVersion = DateTime.UtcNow.AddMinutes(1);
        private readonly long _cloneRowId = 11;
        private readonly int _cloneNumber = 16;
        private readonly bool _cloneLocked = false;
        private readonly int _cloneLayoutColumn = 15;
        private readonly int _cloneLayoutRow = 16;
        private readonly Row _cloneRow = new Row { Id = 21, RowVersion = DateTime.UtcNow, CategoryId = 31, CinemaHallId = 41 };

        protected override void SetProperties(Seat value)
        {
            value.Id = _id;
            value.RowVersion = _rowVersion;
            value.RowId = _rowId;
            value.Number = _number;
            value.Locked = _locked;
            value.LayoutColumn = _layoutColumn;
            value.LayoutRow = _layoutRow;
            value.Row = _row;
        }

        protected override void SetCloneProperties(Seat value)
        {
            value.Id = _cloneId;
            value.RowVersion = _cloneRowVersion;
            value.RowId = _cloneRowId;
            value.Number = _cloneNumber;
            value.Locked = _cloneLocked;
            value.LayoutColumn = _cloneLayoutColumn;
            value.LayoutRow = _cloneLayoutRow;
            value.Row = _cloneRow;
        }

        protected override void CheckProperties(Seat value)
        {
            value.Id.Should().Be(_id);
            value.RowVersion.Should().Be(_rowVersion);
            value.RowId.Should().Be(_rowId);
            value.Number.Should().Be(_number);
            value.Locked.Should().Be(_locked);
            value.LayoutColumn.Should().Be(_layoutColumn);
            value.LayoutRow.Should().Be(_layoutRow);
            value.Row.Should().Be(_row);
        }

        protected override void CheckClonedProperties(Seat value)
        {
            value.Id.Should().Be(_cloneId);
            value.RowVersion.Should().Be(_cloneRowVersion);
            value.RowId.Should().Be(_cloneRowId);
            value.Number.Should().Be(_cloneNumber);
            value.Locked.Should().Be(_cloneLocked);
            value.LayoutColumn.Should().Be(_cloneLayoutColumn);
            value.LayoutRow.Should().Be(_cloneLayoutRow);
            value.Row.Should().Be(_cloneRow);
        }

        protected override int CalculateHashCode()
        {
            return HashCode.Combine(_id, _rowId, _number, _locked, _layoutColumn, _layoutRow);
        }

        protected override string GetAttributeTableName()
        {
            return _attributeTableName;
        }

        protected override void CheckAttributeColumns()
        {
            Attribute_Column_Name_Should(_ => _.RowId, _attributeColumnRowId);
            Attribute_Column_Name_Should(_ => _.Number, _attributeColumnNumber);
            Attribute_Column_Name_Should(_ => _.Locked, _attributeColumnLocked);
            Attribute_Column_Name_Should(_ => _.LayoutColumn, _attributeColumnLayoutColumn);
            Attribute_Column_Name_Should(_ => _.LayoutRow, _attributeColumnLayoutRow);
            Attribute_ColumnRef_Name_Should(_ => _.Row, _attributeColumnRefRow);
        }
    }
}