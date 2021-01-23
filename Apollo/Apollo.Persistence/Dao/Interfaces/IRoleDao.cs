using System.Threading.Tasks;
using Apollo.Domain.Entity;

namespace Apollo.Persistence.Dao.Interfaces
{
    public interface IRoleDao : IBaseDao<Role>
    {
        Task<(bool Exist, long Id)> ExistAsync(string label);
    }
}
