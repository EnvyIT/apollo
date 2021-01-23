using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Apollo.Persistence.Attributes.Base;
using Apollo.Persistence.Dao.Interfaces;

namespace Apollo.Repository.Interfaces
{
    public interface IRepository
    {
        Task ValidateIds<T>(IBaseDao<T> dao, IEnumerable<long> ids) where T : BaseEntity<T>;
        Task ValidateId<T>(IBaseDao<T> dao, long id) where T : BaseEntity<T>;
        Task ValidateNotDeletedById<T>(IBaseDao<T> dao, long id) where T : BaseEntity<T>;
        void ValidateCollection<T>(IEnumerable<T> collection);
        void ValidateDateRange(DateTime from, DateTime to);

        void ValidateNotNull<T>(T entity);
    }
}
