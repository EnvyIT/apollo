using System;
using Apollo.Domain.Entity;
using FluentAssertions;

namespace Apollo.Domain.Test.Entity
{
    public class AddressTest : EntityTest<Address>
    {
        private readonly string _attributeTableName = "address";
        private readonly string _attributeColumnStreet = "street";
        private readonly string _attributeColumnNumber = "number";
        private readonly string _attributeColumnCityId = "city_id";
        private readonly string _attributeColumnRefCity = "city";

        private readonly long _id = 1L;
        private readonly DateTime _rowVersion = DateTime.UtcNow;
        private readonly string _street = "Street";
        private readonly int _number = 10;
        private readonly long _cityId = 15;
        private readonly City _city = new City { Id = 15, Name = "City", PostalCode = "1234", RowVersion = DateTime.UtcNow };

        private readonly long _cloneId = 10L;
        private readonly DateTime _cloneRowVersion = DateTime.UtcNow.AddMinutes(1);
        private readonly string _clonedStreet = "Clone Street";
        private readonly int _clonedNumber = 100;
        private readonly long _clonedCityId = 150;
        private readonly City _clonedCity = new City { Id = 16, Name = "Clone", PostalCode = "4567", RowVersion = DateTime.UtcNow.AddMinutes(15) };

        protected override void SetProperties(Address value)
        {
            value.Id = _id;
            value.RowVersion = _rowVersion;
            value.Street = _street;
            value.Number = _number;
            value.CityId = _cityId;
            value.City = _city;
        }

        protected override void SetCloneProperties(Address value)
        {
            value.Id = _cloneId;
            value.RowVersion = _cloneRowVersion;
            value.Street = _clonedStreet;
            value.Number = _clonedNumber;
            value.CityId = _clonedCityId;
            value.City = _clonedCity;
        }

        protected override void CheckProperties(Address value)
        {
            value.Id.Should().Be(_id);
            value.RowVersion.Should().Be(_rowVersion);
            value.Street.Should().Be(_street);
            value.Number.Should().Be(_number);
            value.CityId.Should().Be(_cityId);
            value.City.Should().Be(_city);
        }

        protected override void CheckClonedProperties(Address value)
        {
            value.Id.Should().Be(_cloneId);
            value.RowVersion.Should().Be(_cloneRowVersion);
            value.Street.Should().Be(_clonedStreet);
            value.Number.Should().Be(_clonedNumber);
            value.CityId.Should().Be(_clonedCityId);
            value.City.Should().Be(_clonedCity);
        }

        protected override int CalculateHashCode()
        {
            return HashCode.Combine(_id, _street, _number, _cityId);
        }

        protected override string GetAttributeTableName()
        {
            return _attributeTableName;
        }

        protected override void CheckAttributeColumns()
        {
            Attribute_Column_Name_Should(_ => _.Street, _attributeColumnStreet);
            Attribute_Column_Name_Should(_ => _.Number, _attributeColumnNumber);
            Attribute_Column_Name_Should(_ => _.CityId, _attributeColumnCityId);
            Attribute_ColumnRef_Name_Should(_ => _.City, _attributeColumnRefCity);
        }
    }
}