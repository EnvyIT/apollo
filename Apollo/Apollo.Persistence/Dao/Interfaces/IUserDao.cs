using System.Collections.Generic;
using System.Threading.Tasks;
using Apollo.Domain.Entity;

namespace Apollo.Persistence.Dao.Interfaces
{
    public interface IUserDao : IBaseDao<User>
    {
        Task<IEnumerable<User>> SelectDeletedAsync();
        Task<User> SelectWithAllReferencesByUuidAsync(string uuid);
        Task<IEnumerable<User>> SelectWithRoleByRoleAsync(IEnumerable<long> roleIds);
        Task<IEnumerable<User>> SelectWithRoleByCityAsync(long cityId);
        Task<Address> SelectAddressWithCityAsync(long userId);
        Task<bool> IsAnyUserReferencingAddressAsync(long addressId);
        Task<bool> IsAnyUserReferencingRoleAsync(long roleId);
        Task<User> SelectUserWithAllReferencesByIdAsync(long id);
    }
}
