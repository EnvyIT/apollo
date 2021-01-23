using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Core.Dto;
using Apollo.Core.Interfaces;
using Apollo.Domain.Entity;
using Apollo.UnitOfWork.Interfaces;
using Apollo.Util.Logger;
using static Apollo.Core.Dto.Mapper;
using static Apollo.Util.ValidationHelper;

namespace Apollo.Core.Implementation
{
    public class UserService : IUserService
    {
        private static readonly IApolloLogger<UserService> Logger = LoggerFactory.CreateLogger<UserService>();
        private readonly IUnitOfWork _unitOfWork;

        public UserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            return (await _unitOfWork.RepositoryUser.GetUsersAsync()).Select(Map);
        }

        public async Task<UserDto> GetUserWithAddressByUuidAsync(string uuid)
        {
            var result = await _unitOfWork.RepositoryUser.GetUserWithAllReferencesByUuidAsync(uuid);
            return result == null ? null : Map(result);
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersByCityIdAsync(long cityId)
        {
            return (await _unitOfWork.RepositoryUser.GetUsersWithRoleByCityAsync(cityId)).Select(Map);
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersByRoleIdAsync(long roleId)
        {
            return (await _unitOfWork.RepositoryUser.GetUsersWithRoleByRoleAsync(roleId)).Select(Map);
        }

        public async Task<AddressDto> GetUserAddressByIdAsync(long userId)
        {
            var result = await _unitOfWork.RepositoryUser.GetUserAddressWithCityAsync(userId);
            return result == null ? null : Map(result);
        }

        public async Task<bool> DeleteUserAsync(long userId)
        {
            var result = true;

            var user = await _unitOfWork.RepositoryUser.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            await _unitOfWork.Transaction().PerformBlock(async () =>
            {
                result = result && await _unitOfWork.RepositoryUser.DeleteAddressAsync(user.AddressId) > 0;
                result = result && await _unitOfWork.RepositoryUser.DeleteUserAsync(userId) > 0;
            }).Commit();

            return result;
        }

        public async Task<UserDto> AddUserAsync(UserDto user)
        {
            ValidateUserDto(user);

            var mappedUser = Map(user);
            var cityExist =
                await _unitOfWork.RepositoryUser.CityExistAsync(user.Address.PostalCode, user.Address.City);

            await _unitOfWork.Transaction().PerformBlock(async () =>
            {
                if (!cityExist.Exist)
                {
                    var cityId = await _unitOfWork.RepositoryUser.AddCityAsync(mappedUser.Address.City);
                    ValidateId<UserService, City>(Logger.Here(), cityId);
                    mappedUser.Address.CityId = cityId;
                }

                var addressId = await _unitOfWork.RepositoryUser.AddAddressAsync(mappedUser.Address);
                ValidateId<UserService, Address>(Logger.Here(), addressId);
                mappedUser.AddressId = addressId;

                var userId = await _unitOfWork.RepositoryUser.AddUserAsync(mappedUser);
                ValidateId< UserService, User>(Logger.Here(), userId);
            }).Commit();

            return user;
        }

        public async Task<UserDto> UpdateUserAsync(UserDto userDto)
        {
            ValidateUserDto(userDto);

            var mappedUser = Map(userDto);
            var cityExist =
                await _unitOfWork.RepositoryUser.CityExistAsync(userDto.Address.PostalCode, userDto.Address.City);

            await _unitOfWork.Transaction().PerformBlock(async () =>
            {
                if (!cityExist.Exist)
                {
                    var cityId = await _unitOfWork.RepositoryUser.AddCityAsync(mappedUser.Address.City);
                    ValidateId<UserService, User>(Logger.Here(), cityId);
                    mappedUser.Address.CityId = cityId;
                }

                var updatedAddress = await _unitOfWork.RepositoryUser.UpdateAddressAsync(mappedUser.Address);
                var updatedUser = await _unitOfWork.RepositoryUser.UpdateUserAsync(mappedUser);
                ValidateUpdate< UserService, Address>(Logger.Here(), updatedAddress);
                ValidateUpdate<UserService, User>(Logger.Here(), updatedUser);
            }).Commit();

            return userDto;
        }

        public async Task<UserDto> GetUserIdAsync(long id)
        {
            var userDto = Map(await _unitOfWork.RepositoryUser.GetUserWithAllReferencesByIdAsync(id));
            ValidateUserDto(userDto);
            return userDto;
        }

        private void ValidateUserDto(UserDto user)
        {
            ValidateNull( Logger.Here(), user);
            ValidateNull( Logger.Here(), user.Address);
            ValidateNull( Logger.Here(), user.Role);
        }

    }
}