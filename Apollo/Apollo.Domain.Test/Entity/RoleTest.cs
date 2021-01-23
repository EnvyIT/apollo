using System;
using Apollo.Domain.Entity;
using FluentAssertions;

namespace Apollo.Domain.Test.Entity
{
    public class RoleTest : EntityTest<Role>
    {
        private readonly string _attributeTableName = "role";
        private readonly string _attributeColumnLabel = "label";
        private readonly string _attributeColumnMaxReservations = "max_reservations";

        private readonly long _id = 1L;
        private readonly DateTime _rowVersion = DateTime.UtcNow;
        private readonly string _label = "Role 1";
        private readonly int _maxReservations = 50;

        private readonly long _cloneId = 10L;
        private readonly DateTime _cloneRowVersion = DateTime.UtcNow.AddMinutes(1);
        private readonly string _cloneLabel = "Role 2";
        private readonly int _cloneMaxReservations = 100;

        protected override void SetProperties(Role value)
        {
            value.Id = _id;
            value.RowVersion = _rowVersion;
            value.Label = _label;
            value.MaxReservations = _maxReservations;
        }

        protected override void SetCloneProperties(Role value)
        {
            value.Id = _cloneId;
            value.RowVersion = _cloneRowVersion;
            value.Label = _cloneLabel;
            value.MaxReservations = _cloneMaxReservations;
        }

        protected override void CheckProperties(Role value)
        {
            value.Id.Should().Be(_id);
            value.RowVersion.Should().Be(_rowVersion);
            value.Label.Should().Be(_label);
            value.MaxReservations.Should().Be(_maxReservations);
        }

        protected override void CheckClonedProperties(Role value)
        {
            value.Id.Should().Be(_cloneId);
            value.RowVersion.Should().Be(_cloneRowVersion);
            value.Label.Should().Be(_cloneLabel);
            value.MaxReservations.Should().Be(_cloneMaxReservations);
        }

        protected override int CalculateHashCode()
        {
            return HashCode.Combine(_id, _label, _maxReservations);
        }

        protected override string GetAttributeTableName()
        {
            return _attributeTableName;
        }

        protected override void CheckAttributeColumns()
        {
            Attribute_Column_Name_Should(_ => _.Label, _attributeColumnLabel);
            Attribute_Column_Name_Should(_ => _.MaxReservations, _attributeColumnMaxReservations);
        }
    }
}