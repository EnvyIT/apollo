using Apollo.Persistence.FluentEntity.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Apollo.Persistence.Attributes.Attributes;

namespace Apollo.Persistence.FluentEntity.Ado
{
    public static class FluentEntityAdoHelper
    {
        private static readonly ISet<Type> NumericTypes;
        private static readonly ISet<Type> FloatingPointTypes;
        private const string RowVersion = "row_version";
        private const string MysqlDateFormat = "yyyy-MM-dd HH:mm:ss";

        static FluentEntityAdoHelper()
        {
            NumericTypes = GetNumericTypes();
            FloatingPointTypes = GetFloatingTypes();
        }

        private static ISet<Type> GetNumericTypes()
        {
            return new HashSet<Type>
            {
                typeof(byte),
                typeof(sbyte),
                typeof(short),
                typeof(ushort),
                typeof(uint),
                typeof(int),
                typeof(ulong),
                typeof(long)
            };
        }

        private static ISet<Type> GetFloatingTypes()
        {
            return new HashSet<Type>
            {
                typeof(decimal),
                typeof(double),
                typeof(float)
            };
        }

        public static void Perform(this IEnumerable<FluentEntityProperty<EntityColumnAttribute>> properties,
            Action<FluentEntityProperty<EntityColumnAttribute>> callback,
            Predicate<FluentEntityProperty<EntityColumnAttribute>> predicate = null)
        {
            foreach (var property in properties)
            {
                if (predicate == null || predicate.Invoke(property))
                {
                    callback?.Invoke(property);
                }
            }
        }

        public static void Perform(this IEnumerable<FluentEntityProperty<EntityColumnAttribute>> properties,
            Action<FluentEntityProperty<EntityColumnAttribute>> callback,
            Action postCallback, 
            Func<FluentEntityProperty<EntityColumnAttribute>, object, bool> predicate,
            object entity)
        {
            foreach (var property in properties)
            {
                if (predicate.Invoke(property, entity))
                {
                    callback?.Invoke(property);
                }
            }
            postCallback?.Invoke();
        }

        private static bool IsRowVersion(FluentEntityProperty<EntityColumnAttribute> property)
        {
            return property.Attribute.Name == RowVersion;
        }

        public static bool IsValueType<TKey>(TKey value)
        {
            return IsBoolean(value) || IsIntegerNumber(value);
        }

        private static bool IsDateTime<TKey>(TKey value)
        {
            return value is DateTime;
        }

        public static bool IsBoolean<TKey>(TKey value)
        {
            return value is bool;
        }

        public static bool IsIntegerNumber<TKey>(TKey value)
        {
            return NumericTypes.Contains(value.GetType());
        }

        public static bool IsFloatingPointNumber<TKey>(TKey value)
        {
            return FloatingPointTypes.Contains(value.GetType());
        }

        public static string JoinEnumerable<T>(IEnumerable<T> data, string divider)
        {
            return string.Join(divider, data);
        }

        public static string ConcatTableColumn(string source, string key)
        {
            return $"{source}.{key}";
        }

        public static string ConcatTableColumnAs(string source, string key)
        {
            return $"{source}{key}";
        }

        public static FluentEntityProperty<EntityColumnAttribute> GetColumnProperty<T, TProperty>(Expression<Func<T, TProperty>> property)
        {
            return GetMetaProperty<T, TProperty, EntityColumnAttribute>(property);
        }

        public static string GetColumnPropertyName<T, TProperty>(Expression<Func<T, TProperty>> property)
        {
            return GetMetaProperty<T, TProperty, EntityColumnAttribute>(property).Attribute.Name;
        }

        public static string GetColumnPropertyNameQualified<T, TProperty>(Expression<Func<T, TProperty>> property)
        {
            var metaData = GetMetaProperty<T, TProperty, EntityColumnAttribute>(property);
            var tableName = GetTableName(property);
            var columnName = metaData.Attribute.Name;
            return ConcatTableColumn(tableName, columnName);
        }

        public static FluentEntityProperty<EntityColumnRefAttribute> GetColumnRefProperty<T, TProperty>(Expression<Func<T, TProperty>> property)
        {
            return GetMetaProperty<T, TProperty, EntityColumnRefAttribute>(property);
        }
        public static IEnumerable<FluentEntityProperty<EntityColumnAttribute>> GetAllColumnProperties(Type type)
        {
            foreach (var propertyInfo in type.GetProperties())
            {
                var attribute = GetCustomAttribute<EntityColumnAttribute>(propertyInfo.GetCustomAttributes(false));
                if (attribute != null)
                {
                    yield return new FluentEntityProperty<EntityColumnAttribute>(attribute, propertyInfo);
                }
            }
        }

        public static string GetTableName<T, TProperty>(Expression<Func<T, TProperty>> key)
        {
            var member = key.Body as MemberExpression;
            return GetTableName(member?.Expression.Type);
        }

        public static string GetTableName(Type type)
        {
            return ((EntityTableAttribute)type.GetCustomAttributes(typeof(EntityTableAttribute), true).FirstOrDefault())?.Source;
        }

        private static FluentEntityProperty<TAttribute> GetMetaProperty<T, TProperty, TAttribute>(Expression<Func<T, TProperty>> property) where TAttribute : class
        {
            var member = property.Body as MemberExpression;
            var propertyInfo = member?.Member as PropertyInfo;
            var attribute = propertyInfo?.GetCustomAttribute(typeof(TAttribute), true) as TAttribute;

            return new FluentEntityProperty<TAttribute>(attribute, propertyInfo);
        }

        private static T GetCustomAttribute<T>(IEnumerable<object> attributes)
        {
            return attributes.Where(attribute => typeof(T) == attribute.GetType())
                    .Select(attribute => (T)attribute)
                    .SingleOrDefault();
        }

        public static bool IsBoolean(FluentEntityProperty<EntityColumnAttribute> column)
        {
            return column.Property.PropertyType == typeof(bool);
        }

        public static string ApplyValueByType<TKey>(TKey value)
        {
            if (IsFloatingPointNumber(value))
            {
                return Convert.ToDecimal(value).ToString(CultureInfo.InvariantCulture);
            }
            if(IsDateTime(value))
            {
                return $"'{Convert.ToDateTime(value).ToString(MysqlDateFormat)}'";
            }
            return IsValueType(value) ? $"{value}" : $"'{value}'";
        }

        
        public static IEnumerable<string> ApplyValuesByType<TKey>(IEnumerable<TKey> values)
        {
            return values.Select(ApplyValueByType);
        }

        private static bool IsIdWithNullValue(FluentEntityProperty<EntityColumnAttribute> property, object value)
        {
            return property.Property.Name.ToLower().Contains("id") && (value == null || IsIntegerNumber(value) && (long)value == 0L);
        }

        public static object GetValue(FluentEntityProperty<EntityColumnAttribute> property, object entity)
        {
            var value = property.Property.GetValue(entity);
            if (IsRowVersion(property))
            {
                value = DateTime.UtcNow;
            }

            if (IsIdWithNullValue(property, value))
            {
                value = null;
            }

            return value;
        }
    }
}
