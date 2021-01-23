using System;
using Apollo.Domain.Entity;
using FluentAssertions;

namespace Apollo.Domain.Test.Entity
{
    public class RowCategoryTest : EntityTest<RowCategory>
    {
        private readonly string _attributeTableName = "row_category";
        private readonly string _attributeColumnName = "name";
        private readonly string _attributeColumnPriceFactor = "price_factor";

        private readonly long _id = 1L;
        private readonly DateTime _rowVersion = DateTime.UtcNow;
        private readonly string _name= "Cat 1";
        private readonly double _priceFactor= 0.5;

        private readonly long _cloneId = 10L;
        private readonly DateTime _cloneRowVersion = DateTime.UtcNow.AddMinutes(1);
        private readonly string _cloneName = "Cat 2";
        private readonly double _clonePriceFactor = 0.75;

        protected override void SetProperties(RowCategory value)
        {
            value.Id = _id;
            value.RowVersion = _rowVersion;
            value.Name = _name;
            value.PriceFactor = _priceFactor;
        }

        protected override void SetCloneProperties(RowCategory value)
        {
            value.Id = _cloneId;
            value.RowVersion = _cloneRowVersion;
            value.Name = _cloneName;
            value.PriceFactor = _clonePriceFactor;
        }

        protected override void CheckProperties(RowCategory value)
        {
            value.Id.Should().Be(_id);
            value.RowVersion.Should().Be(_rowVersion);
            value.Name.Should().Be(_name);
            value.PriceFactor.Should().Be(_priceFactor);
        }

        protected override void CheckClonedProperties(RowCategory value)
        {
            value.Id.Should().Be(_cloneId);
            value.RowVersion.Should().Be(_cloneRowVersion);
            value.Name.Should().Be(_cloneName);
            value.PriceFactor.Should().Be(_clonePriceFactor);
        }

        protected override int CalculateHashCode()
        {
            return HashCode.Combine(_id, _name, _priceFactor);
        }

        protected override string GetAttributeTableName()
        {
            return _attributeTableName;
        }

        protected override void CheckAttributeColumns()
        {
            Attribute_Column_Name_Should(_ => _.Name, _attributeColumnName);
            Attribute_Column_Name_Should(_ => _.PriceFactor, _attributeColumnPriceFactor);
        }
    }
}