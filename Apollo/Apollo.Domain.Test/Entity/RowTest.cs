using System;
using Apollo.Domain.Entity;
using FluentAssertions;

namespace Apollo.Domain.Test.Entity
{
    public class RowTest : EntityTest<Row>
    {
        private readonly string _attributeTableName = "row";
        private readonly string _attributeColumnNumber = "number";
        private readonly string _attributeColumnCategoryId = "category";
        private readonly string _attributeColumnCinemaHallId = "cinema_hall";
        private readonly string _attributeColumnRefCategory = "row_category";
        private readonly string _attributeColumnRefCinemaHall = "cinema_hall";

        private readonly long _id = 1L;
        private readonly DateTime _rowVersion = DateTime.UtcNow;
        private readonly int _number = 10;
        private readonly long _categoryId = 20;
        private readonly long _cinemaHallId = 30;
        private readonly RowCategory _category = new RowCategory
        {
            Id = 40,
            RowVersion = DateTime.UtcNow,
            Name = "Cat 1",
            PriceFactor = 0.5
        };
        private readonly CinemaHall _cinemaHall = new CinemaHall
        {
            Id = 50,
            RowVersion = DateTime.UtcNow,
            Label = "Hall 10"
        };

        private readonly long _cloneId = 10L;
        private readonly DateTime _cloneRowVersion = DateTime.UtcNow.AddMinutes(1);
        private readonly int _cloneNumber = 11;
        private readonly long _cloneCategoryId = 21;
        private readonly long _cloneCinemaHallId = 31;
        private readonly RowCategory _cloneCategory = new RowCategory
        {
            Id = 41,
            RowVersion = DateTime.UtcNow,
            Name = "Cat 2",
            PriceFactor = 0.75
        };
        private readonly CinemaHall _cloneCinemaHall = new CinemaHall
        {
            Id = 51,
            RowVersion = DateTime.UtcNow,
            Label = "Hall 15"
        };

        protected override void SetProperties( Row value)
        {
            value.Id = _id;
            value.RowVersion = _rowVersion;
            value.Number = _number;
            value.CinemaHallId = _cinemaHallId;
            value.CategoryId = _categoryId;
            value.CinemaHall = _cinemaHall;
            value.Category = _category;
        }

        protected override void SetCloneProperties(Row value)
        {
            value.Id = _cloneId;
            value.RowVersion = _cloneRowVersion;
            value.Number = _cloneNumber;
            value.CinemaHallId = _cloneCinemaHallId;
            value.CategoryId = _cloneCategoryId;
            value.CinemaHall = _cloneCinemaHall;
            value.Category = _cloneCategory;
        }

        protected override void CheckProperties(Row value)
        {
            value.Id.Should().Be(_id);
            value.RowVersion.Should().Be(_rowVersion);
            value.Number.Should().Be(_number);
            value.CinemaHallId.Should().Be(_cinemaHallId);
            value.CategoryId.Should().Be(_categoryId);
            value.CinemaHall.Should().Be(_cinemaHall);
            value.Category.Should().Be(_category);
        }

        protected override void CheckClonedProperties(Row value)
        {
            value.Id.Should().Be(_cloneId);
            value.RowVersion.Should().Be(_cloneRowVersion);
            value.Number.Should().Be(_cloneNumber);
            value.CinemaHallId.Should().Be(_cloneCinemaHallId);
            value.CategoryId.Should().Be(_cloneCategoryId);
            value.CinemaHall.Should().Be(_cloneCinemaHall);
            value.Category.Should().Be(_cloneCategory);
        }

        protected override int CalculateHashCode()
        {
            return HashCode.Combine(_id, _number, _cinemaHallId, _categoryId);
        }

        protected override string GetAttributeTableName()
        {
            return _attributeTableName;
        }

        protected override void CheckAttributeColumns()
        {
            Attribute_Column_Name_Should(_ => _.Number, _attributeColumnNumber);
            Attribute_Column_Name_Should(_ => _.CategoryId, _attributeColumnCategoryId);
            Attribute_Column_Name_Should(_ => _.CinemaHallId, _attributeColumnCinemaHallId);
            Attribute_ColumnRef_Name_Should(_ => _.CinemaHall, _attributeColumnRefCinemaHall);
            Attribute_ColumnRef_Name_Should(_ => _.Category, _attributeColumnRefCategory);
        }
    }
}