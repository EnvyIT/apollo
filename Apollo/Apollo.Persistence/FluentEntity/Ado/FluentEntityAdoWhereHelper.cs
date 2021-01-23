using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Apollo.Persistence.Attributes.Base;

namespace Apollo.Persistence.FluentEntity.Ado
{
    public class FluentEntityAdoWhereHelper<T> where T : BaseEntity<T>, new()
    {
        private readonly StringBuilder _commandBuilder;
        private readonly bool _autoReset;

        public string CommandWhere
        {
            get
            {
                var result = _commandBuilder.ToString();
                if (_autoReset)
                {
                    _commandBuilder.Clear();
                }
                return result;
            }
        }

        public FluentEntityAdoWhereHelper(bool autoReset = true)
        {
            _commandBuilder = new StringBuilder();
            _autoReset = autoReset;
        }

        
        public void Where<TKey>(Expression<Func<T, TKey>> key)
        {
            AddWhereCondition(key, "WHERE");
        }

        public void Or<TKey>(Expression<Func<T, TKey>> key)
        {
            AddWhereCondition(key, "OR");
        }
        public void And<TKey>(Expression<Func<T, TKey>> key)
        {
            AddWhereCondition(key, "AND");
        }

        public void Equal<TKey>(TKey value)
        {
            _commandBuilder.Append($" = {FluentEntityAdoHelper.ApplyValueByType(value)}");
        }


        public void GreaterThan<TKey>(TKey value)
        {
            _commandBuilder.Append($" > {FluentEntityAdoHelper.ApplyValueByType(value)}");
        }

        public void LowerThan<TKey>(TKey value)
        {
            _commandBuilder.Append($" < {FluentEntityAdoHelper.ApplyValueByType(value)}");
        }

        public void In<TKey>(IEnumerable<TKey> values)
        {
            _commandBuilder.Append($" IN ({string.Join(", ", FluentEntityAdoHelper.ApplyValuesByType(values))})");
        }

        private void AddWhereCondition<TKey>(Expression<Func<T, TKey>> key, string keyWord)
        {
            _commandBuilder.Append($" {keyWord} {FluentEntityAdoHelper.GetColumnPropertyNameQualified(key)}");
        }

        public void StartsWith<TKey>(TKey value)
        {
            _commandBuilder.Append($" LIKE '{value}%'");
        }

        public void GreaterThanEquals<TKey>(TKey value)
        {
            _commandBuilder.Append($" >= {FluentEntityAdoHelper.ApplyValueByType(value)}");
        }

        public void LowerThanEquals<TKey>(TKey value)
        {
            _commandBuilder.Append($" <= {FluentEntityAdoHelper.ApplyValueByType(value)}");
        }

        public void NotIn<TKey>(IEnumerable<TKey> values)
        {
            _commandBuilder.Append($" NOT IN ({string.Join(", ", FluentEntityAdoHelper.ApplyValuesByType(values))})");
        }
    }

}
