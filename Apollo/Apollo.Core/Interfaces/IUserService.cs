using System.Collections.Generic;
using System.Threading.Tasks;
using Apollo.Core.Dto;

namespace Apollo.Core.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserDto>> GetAllUsersAsync();
        Task<UserDto> GetUserWithAddressByUuidAsync(string uuid);
        Task<IEnumerable<UserDto>> GetAllUsersByCityIdAsync(long cityId);
        Task<IEnumerable<UserDto>> GetAllUsersByRoleIdAsync(long roleId);
        Task<AddressDto> GetUserAddressByIdAsync(long userId);

        Task<bool> DeleteUserAsync(long userId);
        Task<UserDto> AddUserAsync(UserDto user);
        Task<UserDto> UpdateUserAsync(UserDto user);
        Task<UserDto> GetUserIdAsync(long userId);
    }
}
