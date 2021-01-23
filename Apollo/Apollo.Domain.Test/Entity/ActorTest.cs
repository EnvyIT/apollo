using System;
using Apollo.Domain.Entity;
using FluentAssertions;

namespace Apollo.Domain.Test.Entity
{
    public class ActorTest: EntityTest<Actor>
    {
        private readonly string _attributeTableName = "actor";
        private readonly string _attributeColumnFirstName = "first_name";
        private readonly string _attributeColumnLastName = "last_name";

        private readonly long _id = 1L;
        private readonly DateTime _rowVersion = DateTime.UtcNow;
        private readonly string _firstName = "First";
        private readonly string _lastName = "Last";

        private readonly long _cloneId = 10L;
        private readonly DateTime _cloneRowVersion = DateTime.UtcNow.AddMinutes(1);
        private readonly string _cloneFirstName = "Clone";
        private readonly string _cloneLastName = "Name";

        protected override void SetProperties(Actor value)
        {
            value.Id = _id;
            value.RowVersion = _rowVersion;
            value.FirstName = _firstName;
            value.LastName = _lastName;
        }

        protected override void SetCloneProperties(Actor value)
        {
            value.Id = _cloneId;
            value.RowVersion = _cloneRowVersion;
            value.FirstName = _cloneFirstName;
            value.LastName = _cloneLastName;
        }

        protected override void CheckProperties(Actor value)
        {
            value.Id.Should().Be(_id);
            value.RowVersion.Should().Be(_rowVersion);
            value.FirstName.Should().Be(_firstName);
            value.LastName.Should().Be(_lastName);
            value.Name.Should().Be($"{_firstName} {_lastName}");
        }

        protected override void CheckClonedProperties(Actor value)
        {
            value.Id.Should().Be(_cloneId);
            value.RowVersion.Should().Be(_cloneRowVersion);
            value.FirstName.Should().Be(_cloneFirstName);
            value.LastName.Should().Be(_cloneLastName);
            value.Name.Should().Be($"{_cloneFirstName} {_cloneLastName}");
        }

        protected override int CalculateHashCode()
        {
            return HashCode.Combine(_id, _firstName, _lastName);
        }

        protected override string GetAttributeTableName()
        {
            return _attributeTableName;
        }

        protected override void CheckAttributeColumns()
        {
            Attribute_Column_Name_Should(_ => _.FirstName, _attributeColumnFirstName);
            Attribute_Column_Name_Should(_ => _.LastName, _attributeColumnLastName);
        }
    }
}