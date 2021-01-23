using System;
using Apollo.Domain.Entity;
using FluentAssertions;

namespace Apollo.Domain.Test.Entity
{
    public class GenreTest : EntityTest<Genre>
    {
        private readonly string _attributeTableName = "genre";
        private readonly string _attributeColumnName = "name";

        private readonly long _id = 1L;
        private readonly DateTime _rowVersion = DateTime.UtcNow;
        private readonly string _name = "Drama";

        private readonly long _cloneId = 10L;
        private readonly DateTime _cloneRowVersion = DateTime.UtcNow.AddMinutes(1);
        private readonly string _cloneName = "Horror";

        protected override void SetProperties(Genre value)
        {
            value.Id = _id;
            value.RowVersion = _rowVersion;
            value.Name = _name;
        }

        protected override void SetCloneProperties(Genre value)
        {
            value.Id = _cloneId;
            value.RowVersion = _cloneRowVersion;
            value.Name = _cloneName;
        }

        protected override void CheckProperties(Genre value)
        {
            value.Id.Should().Be(_id);
            value.RowVersion.Should().Be(_rowVersion);
            value.Name.Should().Be(_name);
        }

        protected override void CheckClonedProperties(Genre value)
        {
            value.Id.Should().Be(_cloneId);
            value.RowVersion.Should().Be(_cloneRowVersion);
            value.Name.Should().Be(_cloneName);
        }

        protected override int CalculateHashCode()
        {
            return HashCode.Combine(_id, _name);
        }

        protected override string GetAttributeTableName()
        {
            return _attributeTableName;
        }

        protected override void CheckAttributeColumns()
        {
            Attribute_Column_Name_Should(_ => _.Name, _attributeColumnName);
        }
    }
}