using System;
using Apollo.Domain.Entity;
using FluentAssertions;

namespace Apollo.Domain.Test.Entity
{
    public class CityTest : EntityTest<City>
    {
        private readonly string _attributeTableName = "city";
        private readonly string _attributeColumnPostalCode = "postal_code";
        private readonly string _attributeColumnName = "name";

        private readonly long _id = 1L;
        private readonly DateTime _rowVersion = DateTime.UtcNow;
        private readonly string _postalCode = "1234";
        private readonly string _name = "Hagenberg";

        private readonly long _cloneId = 10L;
        private readonly DateTime _cloneRowVersion = DateTime.UtcNow.AddMinutes(1);
        private readonly string _clonePostalCode = "5678";
        private readonly string _cloneName = "Wels";

        protected override void SetProperties(City value)
        {
            value.Id = _id;
            value.RowVersion = _rowVersion;
            value.PostalCode = _postalCode;
            value.Name = _name;
        }

        protected override void SetCloneProperties(City value)
        {
            value.Id = _cloneId;
            value.RowVersion = _cloneRowVersion;
            value.PostalCode = _clonePostalCode;
            value.Name = _cloneName;
        }

        protected override void CheckProperties(City value)
        {
            value.Id.Should().Be(_id);
            value.RowVersion.Should().Be(_rowVersion);
            value.PostalCode.Should().Be(_postalCode);
            value.Name.Should().Be(_name);
        }

        protected override void CheckClonedProperties(City value)
        {
            value.Id.Should().Be(_cloneId);
            value.RowVersion.Should().Be(_cloneRowVersion);
            value.PostalCode.Should().Be(_clonePostalCode);
            value.Name.Should().Be(_cloneName);
        }

        protected override int CalculateHashCode()
        {
            return HashCode.Combine(_id, _postalCode, _name);
        }

        protected override string GetAttributeTableName()
        {
            return _attributeTableName;
        }

        protected override void CheckAttributeColumns()
        {
            Attribute_Column_Name_Should(_ => _.PostalCode, _attributeColumnPostalCode);
            Attribute_Column_Name_Should(_ => _.Name, _attributeColumnName);
        }
    }
}