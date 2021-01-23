using System;
using Apollo.Persistence.Dao;
using System.Collections.Generic;
using Apollo.Persistence.Attributes.Base;
using Apollo.Persistence.Dao.Interfaces;
using Apollo.Persistence.FluentEntity.Interfaces;
using Apollo.Persistence.FluentEntity.Interfaces.Delete;
using Apollo.Persistence.FluentEntity.Interfaces.Insert;
using Apollo.Persistence.FluentEntity.Interfaces.Select;
using Apollo.Persistence.FluentEntity.Interfaces.Shared;
using Apollo.Persistence.FluentEntity.Interfaces.Update;

namespace Apollo.Persistence.FluentEntity.Ado
{
    public class FluentEntityAdo : IFluentEntityFrom
    {
        private readonly IDaoHelper _daoHelper;

        public FluentEntityAdo(IDaoHelper baseDao)
        {
            _daoHelper = baseDao ?? throw new ArgumentNullException(nameof(baseDao));
        }

        public IFluentEntitySelectRequired<T> Select<T>(bool autoReset) where T : BaseEntity<T>, new()
        {
            return new FluentEntityAdoSelect<T>(_daoHelper, false, autoReset);
        }

        public IFluentEntityJoin<T> SelectAll<T>(bool autoReset) where T : BaseEntity<T>, new()
        {
            return new FluentEntityAdoSelect<T>(_daoHelper, true, autoReset);
        }

        public IFluentEntityInsert InsertInto<T>(T value) where T : BaseEntity<T>, new()
        {
            return new FluentEntityAdoInsert<T>(_daoHelper, value);
        }

        public IFluentEntityInsert InsertInto<T>(IEnumerable<T> values) where T : BaseEntity<T>, new()
        {
            return new FluentEntityAdoInsert<T>(_daoHelper, values);
        }

        public IFluentEntityDelete<T> Delete<T>() where T : BaseEntity<T>, new()
        {
            return new FluentEntityAdoDelete<T>(_daoHelper);
        }

        public IFluentEntityUpdate<T> Update<T>(T value) where T : BaseEntity<T>, new()
        {
            return new FluentEntityAdoUpdate<T>(_daoHelper, value);
        }

        public IFluentEntityUpdate<T> Update<T>(IEnumerable<T> values) where T : BaseEntity<T>, new()
        {
            return new FluentEntityAdoUpdate<T>(_daoHelper, values);
        }

    }
}
