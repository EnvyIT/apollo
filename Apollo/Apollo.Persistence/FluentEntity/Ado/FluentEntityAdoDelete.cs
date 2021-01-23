using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Apollo.Persistence.Attributes.Base;
using Apollo.Persistence.Dao;
using Apollo.Persistence.Dao.Interfaces;
using Apollo.Persistence.FluentEntity.Interfaces.Delete;
using Apollo.Persistence.FluentEntity.Interfaces.Shared;

namespace Apollo.Persistence.FluentEntity.Ado
{
    public class FluentEntityAdoDelete<T> : 
        IFluentEntityDelete<T>, 
        IFluentEntityWhereConditionDelete<T>, 
        IFluentEntityWhereConjunctionDelete<T> 
        where T : BaseEntity<T>, new()
    {
        private readonly IDaoHelper _daoHelper;
        private readonly string _baseCommand;
        private readonly FluentEntityAdoWhereHelper<T> _whereHelper;

        public FluentEntityAdoDelete(IDaoHelper daoHelper)
        {
            _daoHelper = daoHelper ?? throw new ArgumentNullException(nameof(daoHelper)); 
            _whereHelper = new FluentEntityAdoWhereHelper<T>();
            _baseCommand = $"DELETE FROM {FluentEntityAdoHelper.GetTableName(typeof(T))}";
        }

        public IFluentEntityWhereConditionDelete<T> Where<TKey>(Expression<Func<T, TKey>> key)
        {
            _whereHelper.Where(key);
            return this;
        }

        public async Task<int> ExecuteAsync()
        { 
            var command = $"{_baseCommand} {_whereHelper.CommandWhere}";
           return await _daoHelper.ExecuteAsync(command);
        }

        public IFluentEntityWhereConjunctionDelete<T> Equal<TKey>(TKey key)
        {
            _whereHelper.Equal(key);
            return this;
        }

        public IFluentEntityWhereConjunctionDelete<T> GreaterThan<TKey>(TKey key)
        {
            _whereHelper.GreaterThan(key);
            return this;
        }

        public IFluentEntityWhereConjunctionDelete<T> LowerThan<TKey>(TKey key)
        {
            _whereHelper.LowerThan(key);
            return this;
        }

        public IFluentEntityWhereConjunctionDelete<T> GreaterThanEquals<TKey>(TKey key)
        {
            _whereHelper.GreaterThanEquals(key);
            return this;
        }

        public IFluentEntityWhereConjunctionDelete<T> LowerThanEquals<TKey>(TKey key)
        {
            _whereHelper.LowerThanEquals(key);
            return this;
        }

        public IFluentEntityWhereConjunctionDelete<T> StartsWith<TKey>(TKey key)
        {
            _whereHelper.StartsWith(key);
            return this;
        }

        public IFluentEntityWhereConjunctionDelete<T> In<TKey>(IEnumerable<TKey> keys)
        {
            _whereHelper.In(keys);
            return this;
        }

        public IFluentEntityWhereConjunctionDelete<T> NotIn<TKey>(IEnumerable<TKey> keys)
        {
            _whereHelper.NotIn(keys);
            return this;
        }

        public IFluentEntityWhereConditionDelete<T> And<TKey>(Expression<Func<T, TKey>> key)
        {
            _whereHelper.And(key);
            return this;
        }

        public IFluentEntityWhereConditionDelete<T> Or<TKey>(Expression<Func<T, TKey>> key)
        {
            _whereHelper.Or(key);
            return this;
        }

        Task ICallExecute.ExecuteAsync()
        {
            return ExecuteAsync();
        }
    }
}
