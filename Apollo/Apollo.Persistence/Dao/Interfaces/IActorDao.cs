using System.Collections.Generic;
using System.Threading.Tasks;
using Apollo.Domain.Entity;

namespace Apollo.Persistence.Dao.Interfaces
{
    public interface IActorDao : IBaseDao<Actor>
    {
        Task<long?> FindByNameAsync(string firstName, string lastName);
    }
}
