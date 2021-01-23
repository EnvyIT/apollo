using System.Threading.Tasks;
using Apollo.Domain.Entity;

namespace Apollo.Persistence.Dao.Interfaces
{
    public interface IAddressDao : IBaseDao<Address>
    {
        Task<bool> IsAnyAddressReferencingCityAsync(long cityId);
    }
}
