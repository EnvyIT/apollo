using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Domain.Entity;
using Apollo.Persistence.Dao.Interfaces;
using Apollo.Persistence.Util;

namespace Apollo.Persistence.Dao.Ado
{
    public class UserDaoAdo : BaseDao<User>, IUserDao
    {
        public UserDaoAdo(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        public Task<IEnumerable<User>> SelectDeletedAsync()
        {
            return FluentSelectAll()
                .Where(_ => _.Deleted)
                .Equal(true)
                .QueryAsync();
        }

        public Task<User> SelectWithAllReferencesByUuidAsync(string uuid)
        {
            return FluentSelectAll()
                .InnerJoin<User, Role, long, long>(_ => _.Role, _ => _.RoleId, _ => _.Id)
                .InnerJoin<User, Address, long, long>(_ => _.Address, _ => _.AddressId, _ => _.Id)
                .InnerJoin<Address, City, long, long>(_ => _.Address.City, _ => _.CityId, _ => _.Id)
                .WhereActive()
                .And(_ => _.Uuid)
                .Equal(uuid)
                .QuerySingleAsync();
        }

        public Task<IEnumerable<User>> SelectWithRoleByRoleAsync(IEnumerable<long> roleIds)
        {
            return FluentSelectAll()
                .InnerJoin<User, Role, long, long>(_ => _.Role, _ => _.RoleId, _ => _.Id)
                .WhereActive()
                .And(_ => _.RoleId)
                .In(roleIds)
                .QueryAsync();
        }

        public Task<IEnumerable<User>> SelectWithRoleByCityAsync(long cityId)
        {
            return FluentSelectAll()
                .InnerJoin<User, Role, long, long>(_ => _.Role, _ => _.RoleId, _ => _.Id)
                .InnerJoin<User, Address, long, long>(null, _ => _.AddressId, _ => _.Id)
                .WhereActive()
                .And(_ => _.Address.CityId)
                .Equal(cityId)
                .QueryAsync();
        }

        public async Task<Address> SelectAddressWithCityAsync(long userId)
        {
            return (await FluentSelectAll()
                    .InnerJoin<User, Address, long, long>(_ => _.Address, _ => _.AddressId, _ => _.Id)
                    .InnerJoin<Address, City, long, long>(_ => _.Address.City, _ => _.CityId, _ => _.Id)
                    .WhereActive()
                    .And(_ => _.Id)
                    .Equal(userId)
                    .QueryAsync())
                ?.FirstOrDefault()?.Address;
        }

        public async Task<bool> IsAnyUserReferencingAddressAsync(long addressId)
        {
            return (await FluentSelectAll()
                    .WhereActive()
                    .And(_ => _.AddressId)
                    .Equal(addressId)
                    .QueryAsync())
                .Any();
        }

        public async Task<bool> IsAnyUserReferencingRoleAsync(long roleId)
        {
            return (await FluentSelectAll()
                    .WhereActive()
                    .And(_ => _.RoleId)
                    .Equal(roleId)
                    .QueryAsync())
                .Any();
        }

        public Task<User> SelectUserWithAllReferencesByIdAsync(long id)
        {
            return FluentSelectAll()
                .InnerJoin<User, Role, long, long>(_ => _.Role, _ => _.RoleId, _ => _.Id)
                .InnerJoin<User, Address, long, long>(_ => _.Address, _ => _.AddressId, _ => _.Id)
                .InnerJoin<Address, City, long, long>(_ => _.Address.City, _ => _.CityId, _ => _.Id)
                .WhereActive()
                .And(_ => _.Id)
                .Equal(id)
                .QuerySingleAsync();
        }
    }
}