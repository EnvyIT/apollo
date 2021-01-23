#nullable enable
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Persistence.Attributes.Base;
using Apollo.Persistence.Dao.Interfaces;
using Apollo.Persistence.FluentEntity;
using Apollo.Persistence.FluentEntity.Interfaces;
using Apollo.Persistence.FluentEntity.Interfaces.Delete;
using Apollo.Persistence.FluentEntity.Interfaces.Insert;
using Apollo.Persistence.FluentEntity.Interfaces.Select;
using Apollo.Persistence.FluentEntity.Interfaces.Shared;
using Apollo.Persistence.FluentEntity.Interfaces.Transaction;
using Apollo.Persistence.FluentEntity.Interfaces.Update;
using Apollo.Persistence.Util;

namespace Apollo.Persistence.Dao
{
    public delegate T RowMapper<T>(IDataRecord record);

    public class BaseDao<T> : IBaseDao<T> where T : BaseEntity<T>, new()
    {
        protected readonly IFluentEntityFrom _fluentEntity;
        protected readonly IFluentTransaction _fluentTransaction;
        protected readonly IFluentEntityDelete<T> _fluentDelete;

        public BaseDao(IConnectionFactory connectionFactory)
        {
            var entityManager = EntityManagerFactory.CreateEntityManager(connectionFactory);
            _fluentEntity = entityManager.FluentEntity();
            _fluentTransaction = entityManager.FluentTransaction();
            _fluentDelete = _fluentEntity.Delete<T>();
        }

        public Task<IEnumerable<T>> SelectAsync(long limit = -1)
        {
            return limit < 0 ? FluentSelectAll().QueryAsync() : FluentSelectAll().Limit(limit).QueryAsync();
        }

        public Task<IEnumerable<T>> SelectActiveAsync(long limit = -1)
        {
            return FluentSelectAll()
                .WhereActive()
                .QueryAsync();
        }

        public Task<T> SelectSingleByIdAsync(long id)
        {
            return FluentSelectAll().Where(_ => _.Id).Equal(id).QuerySingleAsync();
        }

        public Task<T> SelectSingleAsync()
        {
            return FluentSelectAll().QuerySingleAsync();
        }

        public IFluentEntitySelectRequired<T> FluentSelect()
        {
            return _fluentEntity.Select<T>();
        }

        public IFluentEntityInsert FluentInsert(T value)
        {
            return _fluentEntity.InsertInto(value);
        }

        public IFluentEntityInsert FluentInsert(IEnumerable<T> values)
        {
            return _fluentEntity.InsertInto(values);
        }

        public IFluentEntityDelete<T> FluentDelete()
        {
            return _fluentDelete;
        }

        public async Task<int> FluentSoftDeleteById(long id)
        {
            var entity = await SelectSingleByIdAsync(id);
            entity.Deleted = true;
            return await _fluentEntity.Update(entity).ExecuteAsync();
        }

        public IFluentEntityUpdate<T> FluentUpdate(T value)
        {
            return _fluentEntity.Update(value);
        }

        public IFluentEntityUpdate<T> FluentUpdate(IEnumerable<T> values)
        {
            return _fluentEntity.Update(values);
        }

        public IFluentEntityJoin<T> FluentSelectAll()
        {
            return _fluentEntity.SelectAll<T>();
        }

        public IFluentTransaction FluentTransaction()
        {
            return _fluentTransaction;
        }

        public async Task<bool> ExistByIdAsync(long id)
        {
            return (await FluentSelect().Column(_ => _.Id).Where(_ => _.Id).Equal(id).QueryAsync()).Any();
        }

        public async Task<bool> ExistAndNotDeletedByIdAsync(long id)
        {
            return (await FluentSelect().Column(_ => _.Id).WhereActive().And(_ => _.Id).Equal(id).QueryAsync()).Any();
        }

        protected (bool Exist, long Id) CreateIdExistTuple(BaseEntity<T>? entity)
        {
            return (entity != null, entity?.Id ?? 0L);
        }
    }
}