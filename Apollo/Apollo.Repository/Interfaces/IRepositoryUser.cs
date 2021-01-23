using System.Collections.Generic;
using System.Threading.Tasks;
using Apollo.Domain.Entity;

namespace Apollo.Repository.Interfaces
{
    public interface IRepositoryUser : IRepository
    {
        /*
         * Select
         */
         Task<User> GetByIdAsync(long id);
         Task<User> GetUserWithAllReferencesByIdAsync(long id);
         Task<IEnumerable<Role>> GetRolesAsync();
         Task<IEnumerable<User>> GetUsersAsync();
         Task<User> GetUserWithAllReferencesByUuidAsync(string uuid);
         Task<IEnumerable<User>> GetDeletedUsersAsync();
         Task<IEnumerable<User>> GetUsersWithRoleByRoleAsync(long roleId);
         Task<IEnumerable<User>> GetUsersWithRoleByRoleAsync(IEnumerable<long> roleId);
         Task<IEnumerable<User>> GetUsersWithRoleByCityAsync(long cityId);
         Task<Address> GetUserAddressWithCityAsync(long userId);
         Task<IEnumerable<City>> GetCitiesAsync();
         Task<bool> UserExistAsync(long id);
         Task<(bool Exist, long Id)> CityExistAsync(string postalCode, string name);

        /*
         * Add
         */
         Task<long> AddUserAsync(User user);
         Task<long> AddAddressAsync(Address address);
         Task<long> AddCityAsync(City city);
         Task<long> AddRoleAsync(Role role);

        /*
         * Delete
         */
         Task<int> DeleteUserAsync(long userId);
         Task<int> DeleteAddressAsync(long addressId);
         Task<int> DeleteCityAsync(long cityId);
         Task<int> DeleteRoleAsync(long roleId);

        /*
         * Update
         */
         Task<int> UpdateUserAsync(User user);
         Task<int> UpdateAddressAsync(Address address);
         Task<int> UpdateCityAsync(City city);
         Task<int> UpdateRoleAsync(Role role);
    }
}
