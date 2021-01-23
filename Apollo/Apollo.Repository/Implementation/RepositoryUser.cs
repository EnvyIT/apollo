using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Domain.Entity;
using Apollo.Persistence.Dao.Interfaces;
using Apollo.Persistence.Exception;
using Apollo.Persistence.Util;
using Apollo.Repository.Interfaces;
using Apollo.Util.Logger;

namespace Apollo.Repository.Implementation
{
    public class RepositoryUser : Repository, IRepositoryUser
    {
        private static readonly IApolloLogger<RepositoryUser> Logger = LoggerFactory.CreateLogger<RepositoryUser>();
        private readonly IRoleDao _roleDao;
        private readonly IUserDao _userDao;
        private readonly IAddressDao _addressDao;
        private readonly ICityDao _cityDao;

        public RepositoryUser(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
            _roleDao = DaoFactory.CreateRoleDao();
            _userDao = DaoFactory.CreateUserDao();
            _addressDao = DaoFactory.CreateAddressDao();
            _cityDao = DaoFactory.CreateCityDao();
        }

        public async Task<User> GetByIdAsync(long id)
        {
            await ValidateId(_userDao, id);
            return await _userDao.SelectSingleByIdAsync(id);
        }

        public async Task<User> GetUserWithAllReferencesByIdAsync(long id)
        {
            await ValidateId(_userDao, id);
            return await _userDao.SelectUserWithAllReferencesByIdAsync(id);
        }

        public Task<IEnumerable<Role>> GetRolesAsync()
        {
            return _roleDao.SelectActiveAsync();
        }

        public Task<IEnumerable<User>> GetUsersAsync()
        {
            return _userDao.SelectActiveAsync();
        }

        public async Task<User> GetUserWithAllReferencesByUuidAsync(string uuid)
        {
            var result = await _userDao.SelectWithAllReferencesByUuidAsync(uuid);

            if (result != null)
            {
                return result;
            }

            var invalidEntityIdException =
                new InvalidEntityIdException(0L, $"Entity with UUID \"{uuid}\" do not exist!");
            Logger.Error(invalidEntityIdException, "Entity with UUID {uuid} does not exist!", uuid);
            throw invalidEntityIdException;
        }

        public Task<IEnumerable<User>> GetDeletedUsersAsync()
        {
            return _userDao.SelectDeletedAsync();
        }

        public async Task<IEnumerable<User>> GetUsersWithRoleByRoleAsync(long roleId)
        {
            await ValidateId(_roleDao, roleId);
            return await GetUsersWithRoleByRoleAsync(new[] {roleId});
        }

        public async Task<IEnumerable<User>> GetUsersWithRoleByRoleAsync(IEnumerable<long> roleIds)
        {
            ValidateCollection(roleIds);
            await ValidateIds(_roleDao, roleIds);
            return await _userDao.SelectWithRoleByRoleAsync(roleIds);
        }

        public async Task<IEnumerable<User>> GetUsersWithRoleByCityAsync(long cityId)
        {
            await ValidateId(_cityDao, cityId);
            return await _userDao.SelectWithRoleByCityAsync(cityId);
        }

        public async Task<Address> GetUserAddressWithCityAsync(long userId)
        {
            await ValidateId(_userDao, userId);
            await ValidateNotDeletedById(_userDao, userId);
            return await _userDao.SelectAddressWithCityAsync(userId);
        }

        public Task<IEnumerable<City>> GetCitiesAsync()
        {
            return _cityDao.SelectActiveAsync();
        }

        public async Task<bool> UserExistAsync(long id)
        {
            return await _userDao.ExistByIdAsync(id);
        }

        public Task<(bool Exist, long Id)> CityExistAsync(string postalCode, string name)
        {
            return _cityDao.ExistAsync(postalCode, name);
        }

        public async Task<long> AddUserAsync(User user)
        {
            ValidateNotNull(user);
            await ValidateUniqueId(_userDao, user.Id);

            if (user.Address != null && user.AddressId <= 0)
            {
                user.AddressId = user.Address.Id;
            }

            await ValidateId(_addressDao, user.AddressId);

            return (await _userDao.FluentInsert(user).ExecuteAsync()).FirstOrDefault();
        }

        public async Task<long> AddAddressAsync(Address address)
        {
            ValidateNotNull(address);
            await ValidateUniqueId(_addressDao, address.Id);

            if (address.City != null && address.CityId <= 0)
            {
                address.CityId = address.City.Id;
            }

            await ValidateId(_cityDao, address.CityId);

            return (await _addressDao.FluentInsert(address).ExecuteAsync()).FirstOrDefault();
        }

        public async Task<long> AddCityAsync(City city)
        {
            ValidateNotNull(city);
            await ValidateUniqueId(_cityDao, city.Id);
            var cityExist = await CityExistAsync(city.PostalCode, city.Name);
            if (cityExist.Exist)
            {
                var duplicateEntryException = new DuplicateEntryException(cityExist.Id, "City already exist!");
                Logger.Error(duplicateEntryException, "{City} already exists", city);
                throw duplicateEntryException;
            }

            return (await _cityDao.FluentInsert(city).ExecuteAsync()).FirstOrDefault();
        }

        public async Task<long> AddRoleAsync(Role role)
        {
            ValidateNotNull(role);
            await ValidateUniqueId(_roleDao, role.Id);
            var roleExist = await _roleDao.ExistAsync(role.Label);
            if (roleExist.Exist)
            {
                var duplicateEntryException = new DuplicateEntryException(roleExist.Id, "Role label already exist!");
                Logger.Error(duplicateEntryException, "{Role} label already exists", role);
                throw duplicateEntryException;
            }

            return (await _roleDao.FluentInsert(role).ExecuteAsync()).FirstOrDefault();
        }

        public async Task<int> DeleteUserAsync(long userId)
        {
            await ValidateId(_userDao, userId);
            var address = await _userDao.SelectAddressWithCityAsync(userId);
            await _addressDao.FluentSoftDeleteById(address.Id);
            return await _userDao.FluentSoftDeleteById(userId);
        }

        public async Task<int> DeleteAddressAsync(long addressId)
        {
            await ValidateId(_addressDao, addressId);
            if (await _userDao.IsAnyUserReferencingAddressAsync(addressId))
            {
                var entityReferenceException = new EntityReferenceException(addressId, "Address is not deletable");
                Logger.Error(entityReferenceException, "{Address} is not deletable", addressId);
                throw entityReferenceException;
            }

            return await _addressDao.FluentSoftDeleteById(addressId);
        }

        public async Task<int> DeleteCityAsync(long cityId)
        {
            await ValidateId(_cityDao, cityId);
            if (await _addressDao.IsAnyAddressReferencingCityAsync(cityId))
            {
                var entityReferenceException = new EntityReferenceException(cityId, "City is not deletable");
                Logger.Error(entityReferenceException, "{City} is not deletable", cityId);
                throw entityReferenceException;
            }

            return await _cityDao.FluentSoftDeleteById(cityId);
        }

        public async Task<int> DeleteRoleAsync(long roleId)
        {
            await ValidateId(_roleDao, roleId);
            if (await _userDao.IsAnyUserReferencingRoleAsync(roleId))
            {
                var entityReferenceException = new EntityReferenceException(roleId, "Role is not deletable");
                Logger.Error(entityReferenceException, "{Role} is not deletable", roleId);
                throw entityReferenceException;
            }

            return await _roleDao.FluentSoftDeleteById(roleId);
        }

        public async Task<int> UpdateUserAsync(User user)
        {
            ValidateNotNull(user);
            await ValidateId(_userDao, user.Id);

            if (user.Address != null && user.AddressId <= 0)
            {
                user.AddressId = user.Address.Id;
            }

            await ValidateId(_addressDao, user.AddressId);

            return await _userDao.FluentUpdate(user).ExecuteAsync();
        }

        public async Task<int> UpdateAddressAsync(Address address)
        {
            ValidateNotNull(address);
            await ValidateId(_addressDao, address.Id);

            if (address.City != null && address.CityId <= 0)
            {
                address.CityId = address.City.Id;
            }

            await ValidateId(_cityDao, address.CityId);

            return await _addressDao.FluentUpdate(address).ExecuteAsync();
        }

        public async Task<int> UpdateCityAsync(City city)
        {
            ValidateNotNull(city);
            await ValidateId(_cityDao, city.Id);
            return await _cityDao.FluentUpdate(city).ExecuteAsync();
        }

        public async Task<int> UpdateRoleAsync(Role role)
        {
            ValidateNotNull(role);
            await ValidateId(_roleDao, role.Id);
            return await _roleDao.FluentUpdate(role).ExecuteAsync();
        }
    }
}