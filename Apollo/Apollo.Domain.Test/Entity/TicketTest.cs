using System;
using Apollo.Domain.Entity;
using FluentAssertions;

namespace Apollo.Domain.Test.Entity
{
    public class TicketTest : EntityTest<Ticket>
    {
        private readonly string _attributeTableName = "ticket";
        private readonly string _attributeColumnPrinted = "printed";

        private readonly long _id = 1L;
        private readonly DateTime _rowVersion = DateTime.UtcNow;
        private readonly DateTime _printed = DateTime.UtcNow;

        private readonly long _cloneId = 10L;
        private readonly DateTime _cloneRowVersion = DateTime.UtcNow.AddMinutes(1);
        private readonly DateTime _clonePrinted = DateTime.UtcNow.AddMinutes(1);

        protected override void SetProperties(Ticket value)
        {
            value.Id = _id;
            value.RowVersion = _rowVersion;
            value.Printed = _printed;
        }

        protected override void SetCloneProperties(Ticket value)
        {
            value.Id = _cloneId;
            value.RowVersion = _cloneRowVersion;
            value.Printed = _clonePrinted;
        }

        protected override void CheckProperties(Ticket value)
        {
            value.Id.Should().Be(_id);
            value.RowVersion.Should().Be(_rowVersion);
            value.Printed.Should().Be(_printed);
        }

        protected override void CheckClonedProperties(Ticket value)
        {
            value.Id.Should().Be(_cloneId);
            value.RowVersion.Should().Be(_cloneRowVersion);
            value.Printed.Should().Be(_clonePrinted);
        }

        protected override int CalculateHashCode()
        {
            return HashCode.Combine(_id, _printed);
        }

        protected override string GetAttributeTableName()
        {
            return _attributeTableName;
        }

        protected override void CheckAttributeColumns()
        {
            Attribute_Column_Name_Should(_ => _.Printed, _attributeColumnPrinted);
        }
    }
}
