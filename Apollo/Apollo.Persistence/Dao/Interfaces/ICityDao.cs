using System.Threading.Tasks;
using Apollo.Domain.Entity;

namespace Apollo.Persistence.Dao.Interfaces
{
    public interface ICityDao : IBaseDao<City>
    {
        Task<(bool Exist, long Id)> ExistAsync(string postalCode, string name);
    }
}
