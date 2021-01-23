using System.Collections.Generic;
using System.Threading.Tasks;
using Apollo.Persistence.FluentEntity.Interfaces;
using Apollo.Persistence.FluentEntity.Interfaces.Delete;
using Apollo.Persistence.FluentEntity.Interfaces.Insert;
using Apollo.Persistence.FluentEntity.Interfaces.Select;
using Apollo.Persistence.FluentEntity.Interfaces.Transaction;
using Apollo.Persistence.FluentEntity.Interfaces.Update;

namespace Apollo.Persistence.Dao.Interfaces
{
    public interface IBaseDao<T>
    {
        Task<IEnumerable<T>> SelectAsync(long limit = -1);

        Task<IEnumerable<T>> SelectActiveAsync(long limit = -1);

        Task<T> SelectSingleByIdAsync(long id);

        Task<T> SelectSingleAsync();

        IFluentEntitySelectRequired<T> FluentSelect();


        IFluentEntityInsert FluentInsert(T value);
        IFluentEntityInsert FluentInsert(IEnumerable<T> values);

        IFluentEntityUpdate<T> FluentUpdate(T value);
        IFluentEntityUpdate<T> FluentUpdate(IEnumerable<T> values);


        IFluentEntityDelete<T> FluentDelete();
        Task<int> FluentSoftDeleteById(long id);

        IFluentEntityJoin<T> FluentSelectAll();

        IFluentTransaction FluentTransaction();

        Task<bool> ExistByIdAsync(long id);

        Task<bool> ExistAndNotDeletedByIdAsync(long id);
    }
}