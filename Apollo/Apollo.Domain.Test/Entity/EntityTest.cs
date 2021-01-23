using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Apollo.Persistence.Attributes.Attributes;
using Apollo.Persistence.Attributes.Base;
using FluentAssertions;
using NUnit.Framework;

namespace Apollo.Domain.Test.Entity
{
    public abstract class EntityTest<T> where T : BaseEntity<T>, new()
    {
        private readonly string _attributeColumnId = "id";
        private readonly string _attributeColumnRowVersion = "row_version";

        private T _entity;

        protected abstract void SetProperties(T value);

        protected abstract void SetCloneProperties(T value);

        protected abstract void CheckProperties(T value);

        protected abstract void CheckClonedProperties(T value);

        protected abstract int CalculateHashCode();

        protected abstract string GetAttributeTableName();

        protected abstract void CheckAttributeColumns();

        protected void Attribute_Column_Name_Should<TProperty>(Expression<Func<T, TProperty>> property, string name, bool isPrimary = false)
        {
            var attribute = GetAttributeColumn<TProperty, EntityColumnAttribute>(property);
            attribute.Name.Should().Be(name);
            attribute.IsKey.Should().Be(isPrimary);
        }

        protected void Attribute_ColumnRef_Name_Should<TProperty>(Expression<Func<T, TProperty>> property, string expected)
        {
            var attribute = GetAttributeColumn<TProperty, EntityColumnRefAttribute>(property);
            attribute.Name.Should().Be(expected);
        }

        private static TAttribute GetAttributeColumn<TProperty, TAttribute>(Expression<Func<T, TProperty>> property) where TAttribute : class
        {
            var member = property.Body as MemberExpression;
            var propertyInfo = member?.Member as PropertyInfo;
            return propertyInfo?.GetCustomAttribute(typeof(TAttribute), true) as TAttribute;
        }

        [SetUp]
        public void Setup()
        {
            _entity = new T();
        }

        [Test]
        public void Test_Properties()
        {
            SetProperties(_entity);
            CheckProperties(_entity);
        }

        [Test]
        public void Test_Clone()
        {
            SetProperties(_entity);
            var clone = (T)_entity.Clone();

            SetCloneProperties(clone);

            CheckProperties(_entity);
            CheckClonedProperties(clone);
        }

        [Test]
        public void Test_Equals_Same_Concrete()
        {
            _entity.Equals(_entity).Should().BeTrue();
        }

        [Test]
        public void Test_Equals_Same_Object()
        {
            _entity.Equals((object)_entity).Should().BeTrue();
        }

        [Test]
        public void Test_Equals_ClonedObject()
        {
            _entity.Should().Be(_entity.Clone());
        }

        [Test]
        public void Test_Equals_NotEqual()
        {
            var clone = (T)_entity.Clone();
            SetCloneProperties(clone);

            _entity.Should().NotBe(clone);
        }

        [Test]
        public void Test_Equals_Not_Concrete_Null()
        {
            _entity.Equals(null).Should().BeFalse();
        }

        [Test]
        public void Test_Equals_Not_Object_Null()
        {
            _entity.Equals((object)null).Should().BeFalse();
        }

        [Test]
        public void Test_Equals_Not_InvalidType()
        {
            _entity.Equals("invalid").Should().BeFalse();
        }

        [Test]
        public void Test_HashCode()
        {
            SetProperties(_entity);
            _entity.GetHashCode().Should().Be(CalculateHashCode());
        }

        [Test]
        public void Test_Attribute_TableName()
        {
            var tableName = ((EntityTableAttribute)typeof(T)
                .GetCustomAttributes(typeof(EntityTableAttribute), true)
                .FirstOrDefault())?.Source;
            tableName.Should().Be(GetAttributeTableName());
        }

        [Test]
        public void Test_Attribute_ColumnNames()
        {
            Attribute_Column_Name_Should(_ => _.Id, _attributeColumnId, true);
            Attribute_Column_Name_Should(_ => _.RowVersion, _attributeColumnRowVersion);
            CheckAttributeColumns();
        }
    }
}