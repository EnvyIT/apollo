using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Core.Dto;
using Apollo.Core.Exception;
using Apollo.Core.Interfaces;
using Apollo.Domain.Entity;
using Apollo.Persistence.Exception;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace Apollo.Core.Test
{
    public class UserServiceTest
    {
        private MockingHelper _mockingHelper;
        private IUserService _userService;

        private static readonly List<Role> Roles = new List<Role>
        {
            new Role {Id = 1L, Label = "Role 1", MaxReservations = 10},
            new Role {Id = 2L, Label = "Role 2", MaxReservations = 20}
        };

        private static readonly List<City> Cities = new List<City>
        {
            new City {Id = 100L, PostalCode = "1234", Name = "City 1"},
            new City {Id = 200L, PostalCode = "6789", Name = "City 2"}
        };

        private static readonly List<User> Users = new List<User>
        {
            new User
            {
                Id = 1L, Uuid = "724f75e6-71ce-45b0-8bb6-d3908cee727e", Role = Roles[0],
                FirstName = "First 1", LastName = "Last 1", Email = "Mail 1", Phone = "1234",
                Address = new Address
                {
                    Id = 10L, Street = "Street 1", Number = 10, City = Cities[0]
                }
            },
            new User
            {
                Id = 2L, Uuid = "f7f57b3d-890f-4ba8-9c86-278c68ec1dad", Role = Roles[0], FirstName = "First 2",
                LastName = "Last 2", Email = "Mail 2", Phone = "2345",
                Address = new Address
                {
                    Id = 20L, Street = "Street 2", Number = 20, City = Cities[0]
                }
            },
            new User
            {
                Id = 3L, Uuid = "b008704f-4bab-4f89-858b-dcd0c4a7ada1", Role = Roles[1],
                FirstName = "First 3", LastName = "Last 3", Email = "Mail 3", Phone = "3456",
                Address = new Address
                {
                    Id = 30L, Street = "Street 3", Number = 30, City = Cities[1]
                }
            }
        };

        [SetUp]
        public void Setup()
        {
            _mockingHelper = new MockingHelper();
            _userService = _mockingHelper.ServiceFactory.Object.CreateUserService();
        }

        [Test]
        public async Task Test_GetAllUsers()
        {
            _mockingHelper.RepositoryUser.Setup(_ => _.GetUsersAsync()).ReturnsAsync(Users);
            var result = (await _userService.GetAllUsersAsync()).OrderBy(_ => _.Id).ToList();

            ValidateUsers(Users, result);
        }

        [Test]
        public async Task Test_GetUserWithAddressByUuid_InvalidId_Should_ReturnNull()
        {
            const string uuid = "invalid";
            _mockingHelper.RepositoryUser.Setup(_ => _.GetUserWithAllReferencesByUuidAsync(uuid))
                .ReturnsAsync((User) null);

            var result = await _userService.GetUserWithAddressByUuidAsync(uuid);
            result.Should().BeNull();
        }

        [Test]
        public async Task Test_GetUserWithAddressByUuid()
        {
            var uuid = Users.First().Uuid;
            var expectedUser = Users.First(u => u.Uuid == uuid);
            _mockingHelper.RepositoryUser.Setup(_ => _.GetUserWithAllReferencesByUuidAsync(uuid))
                .ReturnsAsync(expectedUser);

            var result = await _userService.GetUserWithAddressByUuidAsync(uuid);
            ValidateUsers(new List<User> {expectedUser}, new List<UserDto> {result});
        }

        [Test]
        public async Task Test_GetAllUsersByCityId_InvalidId_Should_ReturnEmptyList()
        {
            const long cityId = 500L;
            _mockingHelper.RepositoryUser.Setup(_ => _.GetUsersWithRoleByCityAsync(cityId))
                .ReturnsAsync(new List<User>());

            var result = (await _userService.GetAllUsersByCityIdAsync(cityId)).OrderBy(_ => _.Id).ToList();
            result.Should().HaveCount(0);
        }

        [Test]
        public async Task Test_GetAllUsersByCityId()
        {
            var cityId = Cities.First().Id;
            var expectedUsers = Users.Where(user => user.Address.CityId == cityId).ToList();
            _mockingHelper.RepositoryUser.Setup(_ => _.GetUsersWithRoleByCityAsync(cityId))
                .ReturnsAsync(expectedUsers);

            var result = (await _userService.GetAllUsersByCityIdAsync(cityId)).OrderBy(_ => _.Id).ToList();
            ValidateUsers(expectedUsers, result);
        }

        [Test]
        public async Task Test_GetAllUsersByRole_InvalidId_Should_Return_EmptyList()
        {
            var roleId = 500L;
            _mockingHelper.RepositoryUser.Setup(_ => _.GetUsersWithRoleByRoleAsync(roleId))
                .ReturnsAsync(new List<User>());

            var result = (await _userService.GetAllUsersByRoleIdAsync(roleId)).OrderBy(_ => _.Id).ToList();
            result.Should().HaveCount(0);
        }

        [Test]
        public async Task Test_GetAllUsersByRole()
        {
            var roleId = Roles.First().Id;
            var expectedUsers = Users.Where(user => user.RoleId == roleId).ToList();
            _mockingHelper.RepositoryUser.Setup(_ => _.GetUsersWithRoleByRoleAsync(roleId))
                .ReturnsAsync(expectedUsers);

            var result = (await _userService.GetAllUsersByRoleIdAsync(roleId)).OrderBy(_ => _.Id).ToList();
            ValidateUsers(expectedUsers, result);
        }

        [Test]
        public async Task Test_GetUserAddressById_InvalidId_Should_ReturnNull()
        {
            var userId = 500L;
            _mockingHelper.RepositoryUser.Setup(_ => _.GetUserAddressWithCityAsync(userId))
                .ReturnsAsync((Address) null);

            var result = await _userService.GetUserAddressByIdAsync(userId);
            result.Should().BeNull();
        }

        [Test]
        public async Task Test_GetUserAddressById()
        {
            var userId = Users.First().Id;
            var expectedAddress = Users.First().Address;

            _mockingHelper.RepositoryUser.Setup(_ => _.GetUserAddressWithCityAsync(userId))
                .ReturnsAsync(expectedAddress);

            var result = await _userService.GetUserAddressByIdAsync(userId);
            ValidateAddress(expectedAddress, result);
        }

        [Test]
        public async Task Test_DeleteUser_InvalidId_Should_ReturnFalse()
        {
            var userId = 500L;
            _mockingHelper.RepositoryUser.Setup(_ => _.GetByIdAsync(userId)).ReturnsAsync((User) null);

            var result = await _userService.DeleteUserAsync(userId);
            result.Should().BeFalse();
        }

        [Test]
        public async Task Test_DeleteUser_Invalid_IntermediateResults_ShouldReturn_False()
        {
            var user = Users.First();
            var userId = user.Id;

            _mockingHelper.RepositoryUser.Setup(_ => _.GetByIdAsync(userId)).ReturnsAsync(user);
            _mockingHelper.RepositoryUser.Setup(_ => _.DeleteUserAsync(userId)).ReturnsAsync(0);
            _mockingHelper.RepositoryUser.Setup(_ => _.DeleteAddressAsync(userId)).ReturnsAsync(1);

            var result = await _userService.DeleteUserAsync(userId);
            result.Should().BeFalse();

            _mockingHelper.RepositoryUser.Setup(_ => _.DeleteUserAsync(userId)).ReturnsAsync(1);
            _mockingHelper.RepositoryUser.Setup(_ => _.DeleteAddressAsync(userId)).ReturnsAsync(0);

            result = await _userService.DeleteUserAsync(userId);
            result.Should().BeFalse();
        }

        [Test]
        public async Task Test_DeleteUser()
        {
            var user = Users.First();
            var userId = user.Id;

            _mockingHelper.RepositoryUser.Setup(_ => _.GetByIdAsync(userId)).ReturnsAsync(user);
            _mockingHelper.RepositoryUser.Setup(_ => _.DeleteUserAsync(userId)).ReturnsAsync(1);
            _mockingHelper.RepositoryUser.Setup(_ => _.DeleteAddressAsync(user.AddressId)).ReturnsAsync(1);

            var result = await _userService.DeleteUserAsync(userId);
            result.Should().BeTrue();

            _mockingHelper.RepositoryUser.Verify(_ => _.DeleteUserAsync(userId), Times.Once);
            _mockingHelper.RepositoryUser.Verify(_ => _.DeleteAddressAsync(user.AddressId), Times.Once);
            _mockingHelper.RepositoryUser.Verify(_ => _.DeleteCityAsync(user.Address.CityId), Times.Never);
            _mockingHelper.FluentTransactionCommit.Verify(_ => _.Commit(), Times.Once);
        }

        [Test]
        public async Task Test_DeleteUser_TransactionError_ShouldAbort()
        {
            var user = Users.First();
            var userId = user.Id;

            _mockingHelper.RepositoryUser.Setup(_ => _.GetByIdAsync(userId)).ReturnsAsync(user);
            _mockingHelper.RepositoryUser.Setup(_ => _.DeleteAddressAsync(user.AddressId))
                .ThrowsAsync(new PersistenceException(""));

            Func<Task> callback = async () => await _userService.DeleteUserAsync(userId);
            await callback.Should().ThrowAsync<PersistenceException>();

            _mockingHelper.RepositoryUser.Verify(_ => _.DeleteUserAsync(userId), Times.Never);
            _mockingHelper.RepositoryUser.Verify(_ => _.DeleteAddressAsync(user.AddressId), Times.Once);
            _mockingHelper.RepositoryUser.Verify(_ => _.DeleteCityAsync(user.Address.CityId), Times.Never);
        }

        [Test]
        public async Task Test_AddUser_MissingArguments_ShouldThrow()
        {
            Func<Task> callback = async () => await _userService.AddUserAsync(null);
            await callback.Should().ThrowAsync<ArgumentNullException>();

            var user = Mapper.Map(Users.First());
            user.Address = null;

            callback = async () => await _userService.AddUserAsync(user);
            await callback.Should().ThrowAsync<ArgumentNullException>();

            user = Mapper.Map(Users.First());
            user.Role = null;

            callback = async () => await _userService.AddUserAsync(user);
            await callback.Should().ThrowAsync<ArgumentNullException>();
        }

        [Test]
        public async Task Test_AddUser_City_AlreadyExist()
        {
            var user = Users.First();
            var city = user.Address.City;
            var userDto = Mapper.Map(user);

            _mockingHelper.RepositoryUser.Setup(_ => _.AddUserAsync(It.IsAny<User>())).ReturnsAsync(1);
            _mockingHelper.RepositoryUser.Setup(_ => _.AddAddressAsync(It.IsAny<Address>())).ReturnsAsync(1);
            _mockingHelper.RepositoryUser.Setup(_ => _.AddCityAsync(It.IsAny<City>())).ReturnsAsync(1);
            _mockingHelper.RepositoryUser.Setup(_ => _.CityExistAsync(city.PostalCode, city.Name))
                .ReturnsAsync((true, city.Id));

            var result = await _userService.AddUserAsync(userDto);
            result.Should().Be(userDto);

            _mockingHelper.RepositoryUser.Verify(_ => _.AddUserAsync(It.IsAny<User>()), Times.Once);
            _mockingHelper.RepositoryUser.Verify(_ => _.AddAddressAsync(It.IsAny<Address>()), Times.Once);
            _mockingHelper.RepositoryUser.Verify(_ => _.AddCityAsync(It.IsAny<City>()), Times.Never);
            _mockingHelper.FluentTransactionCommit.Verify(_ => _.Commit(), Times.Once);
        }

        [Test]
        public async Task Test_AddUser_City_NotExist()
        {
            var user = Users.First();
            var city = user.Address.City;
            var userDto = Mapper.Map(user);

            _mockingHelper.RepositoryUser.Setup(_ => _.AddUserAsync(It.IsAny<User>())).ReturnsAsync(1);
            _mockingHelper.RepositoryUser.Setup(_ => _.AddAddressAsync(It.IsAny<Address>())).ReturnsAsync(1);
            _mockingHelper.RepositoryUser.Setup(_ => _.AddCityAsync(It.IsAny<City>())).ReturnsAsync(1);
            _mockingHelper.RepositoryUser.Setup(_ => _.CityExistAsync(city.PostalCode, city.Name))
                .ReturnsAsync((false, 0L));

            var result = await _userService.AddUserAsync(userDto);
            result.Should().Be(userDto);

            _mockingHelper.RepositoryUser.Verify(_ => _.AddUserAsync(It.IsAny<User>()), Times.Once);
            _mockingHelper.RepositoryUser.Verify(_ => _.AddAddressAsync(It.IsAny<Address>()), Times.Once);
            _mockingHelper.RepositoryUser.Verify(_ => _.AddCityAsync(It.IsAny<City>()), Times.Once);
            _mockingHelper.FluentTransactionCommit.Verify(_ => _.Commit(), Times.Once);
        }

        [Test]
        public async Task Test_AddUser_Invalid_IntermediateResults_ShouldThrow()
        {
            var user = Users.First();
            var city = user.Address.City;
            var userDto = Mapper.Map(user);

            _mockingHelper.RepositoryUser.Setup(_ => _.CityExistAsync(city.PostalCode, city.Name))
                .ReturnsAsync((false, 0L));

            _mockingHelper.RepositoryUser.Setup(_ => _.AddUserAsync(user)).ReturnsAsync(0L);
            _mockingHelper.RepositoryUser.Setup(_ => _.AddAddressAsync(user.Address)).ReturnsAsync(1L);
            _mockingHelper.RepositoryUser.Setup(_ => _.AddCityAsync(user.Address.City)).ReturnsAsync(1L);

            Func<Task> callback = () => _userService.AddUserAsync(userDto);
            await callback.Should().ThrowAsync<PersistenceException>();

            _mockingHelper.RepositoryUser.Setup(_ => _.AddUserAsync(user)).ReturnsAsync(1L);
            _mockingHelper.RepositoryUser.Setup(_ => _.AddAddressAsync(user.Address)).ReturnsAsync(0L);
            _mockingHelper.RepositoryUser.Setup(_ => _.AddCityAsync(user.Address.City)).ReturnsAsync(1L);

            callback = () => _userService.AddUserAsync(userDto);
            await callback.Should().ThrowAsync<PersistenceException>();

            _mockingHelper.RepositoryUser.Setup(_ => _.AddUserAsync(user)).ReturnsAsync(1L);
            _mockingHelper.RepositoryUser.Setup(_ => _.AddAddressAsync(user.Address)).ReturnsAsync(1L);
            _mockingHelper.RepositoryUser.Setup(_ => _.AddCityAsync(user.Address.City)).ReturnsAsync(0L);

            callback = () => _userService.AddUserAsync(userDto);
            await callback.Should().ThrowAsync<PersistenceException>();
        }

        [Test]
        public async Task Test_UpdateUser_MissingArguments_ShouldThrow()
        {
            Func<Task> callback = async () => await _userService.UpdateUserAsync(null);
            await callback.Should().ThrowAsync<ArgumentNullException>();

            var user = Mapper.Map(Users.First());
            user.Address = null;

            callback = async () => await _userService.UpdateUserAsync(user);
            await callback.Should().ThrowAsync<ArgumentNullException>();

            user = Mapper.Map(Users.First());
            user.Role = null;

            callback = async () => await _userService.UpdateUserAsync(user);
            await callback.Should().ThrowAsync<ArgumentNullException>();
        }

        [Test]
        public async Task Test_UpdateUser_InvalidId_ShouldThrow()
        {
            var user = Users.First();
            _mockingHelper.RepositoryUser.Setup(_ => _.GetUserWithAllReferencesByIdAsync(It.IsAny<long>())).ReturnsAsync(Users.First());
            _mockingHelper.RepositoryUser.Setup(_ => _.UpdateUserAsync(user))
                .ThrowsAsync(new InvalidEntityIdException(user.Id));

            Func<Task> callback = async () => await _userService.UpdateUserAsync(Mapper.Map(user));
            await callback.Should().ThrowAsync<PersistenceException>();
        }

        [Test]
        public async Task Test_UpdateUser_City_AlreadyExist()
        {
            var user = Users.First();
            var city = user.Address.City;
            var userDto = Mapper.Map(user);
            _mockingHelper.RepositoryUser.Setup(_ => _.GetUserWithAllReferencesByIdAsync(It.IsAny<long>())).ReturnsAsync(Users.First());
            _mockingHelper.RepositoryUser.Setup(_ => _.UpdateUserAsync(It.IsAny<User>())).ReturnsAsync(1);
            _mockingHelper.RepositoryUser.Setup(_ => _.UpdateAddressAsync(It.IsAny<Address>())).ReturnsAsync(1);
            _mockingHelper.RepositoryUser.Setup(_ => _.CityExistAsync(city.PostalCode, city.Name))
                .ReturnsAsync((true, city.Id));

            var result = await _userService.UpdateUserAsync(userDto);
            result.Should().Be(userDto);

            _mockingHelper.RepositoryUser.Verify(_ => _.UpdateUserAsync(It.IsAny<User>()), Times.Once);
            _mockingHelper.RepositoryUser.Verify(_ => _.UpdateAddressAsync(It.IsAny<Address>()), Times.Once);
            _mockingHelper.RepositoryUser.Verify(_ => _.UpdateCityAsync(user.Address.City), Times.Never);
            _mockingHelper.FluentTransactionCommit.Verify(_ => _.Commit(), Times.Once);
        }

        [Test]
        public async Task Test_UpdateUser_City_NotExist()
        {
            var user = Users.First();
            var city = user.Address.City;
            var userDto = Mapper.Map(user);
            _mockingHelper.RepositoryUser.Setup(_ => _.GetUserWithAllReferencesByIdAsync(It.IsAny<long>())).ReturnsAsync(Users.First());
            _mockingHelper.RepositoryUser.Setup(_ => _.UpdateUserAsync(It.IsAny<User>())).ReturnsAsync(1);
            _mockingHelper.RepositoryUser.Setup(_ => _.UpdateAddressAsync(It.IsAny<Address>())).ReturnsAsync(1);
            _mockingHelper.RepositoryUser.Setup(_ => _.AddCityAsync(It.IsAny<City>())).ReturnsAsync(1L);
            _mockingHelper.RepositoryUser.Setup(_ => _.CityExistAsync(city.PostalCode, city.Name))
                .ReturnsAsync((false, 0L));

            var result = await _userService.UpdateUserAsync(userDto);
            result.Should().Be(userDto);

            _mockingHelper.RepositoryUser.Verify(_ => _.UpdateUserAsync(It.IsAny<User>()), Times.Once);
            _mockingHelper.RepositoryUser.Verify(_ => _.UpdateAddressAsync(It.IsAny<Address>()), Times.Once);
            _mockingHelper.RepositoryUser.Verify(_ => _.AddCityAsync(It.IsAny<City>()), Times.Once);
            _mockingHelper.FluentTransactionCommit.Verify(_ => _.Commit(), Times.Once);
        }

        [Test]
        public async Task Test_UpdateUser_Invalid_IntermediateResults_ShouldThrow()
        {
            var user = Users.First();
            var city = user.Address.City;
            var userDto = Mapper.Map(user);
            _mockingHelper.RepositoryUser.Setup(_ => _.GetUserWithAllReferencesByIdAsync(It.IsAny<long>())).ReturnsAsync(Users.First());
            _mockingHelper.RepositoryUser.Setup(_ => _.CityExistAsync(city.PostalCode, city.Name))
                .ReturnsAsync((false, 0L));

            _mockingHelper.RepositoryUser.Setup(_ => _.UpdateUserAsync(It.IsAny<User>())).ReturnsAsync(0);
            _mockingHelper.RepositoryUser.Setup(_ => _.UpdateAddressAsync(It.IsAny<Address>())).ReturnsAsync(1);
            _mockingHelper.RepositoryUser.Setup(_ => _.AddCityAsync(It.IsAny<City>())).ReturnsAsync(1L);

            Func<Task> callback = () => _userService.UpdateUserAsync(userDto);
            await callback.Should().ThrowAsync<PersistenceException>();

            _mockingHelper.RepositoryUser.Setup(_ => _.UpdateUserAsync(It.IsAny<User>())).ReturnsAsync(1);
            _mockingHelper.RepositoryUser.Setup(_ => _.UpdateAddressAsync(It.IsAny<Address>())).ReturnsAsync(0);
            _mockingHelper.RepositoryUser.Setup(_ => _.AddCityAsync(It.IsAny<City>())).ReturnsAsync(1L);

            callback = () => _userService.UpdateUserAsync(userDto);
            await callback.Should().ThrowAsync<PersistenceException>();

            _mockingHelper.RepositoryUser.Setup(_ => _.UpdateUserAsync(It.IsAny<User>())).ReturnsAsync(1);
            _mockingHelper.RepositoryUser.Setup(_ => _.UpdateAddressAsync(It.IsAny<Address>())).ReturnsAsync(1);
            _mockingHelper.RepositoryUser.Setup(_ => _.AddCityAsync(It.IsAny<City>())).ReturnsAsync(0);

            callback = () => _userService.UpdateUserAsync(userDto);
            await callback.Should().ThrowAsync<PersistenceException>();
        }

        private void ValidateUsers(List<User> expectedUsers, List<UserDto> users)
        {
            users.Should().HaveCount(expectedUsers.Count);

            for (int i = 0; i < expectedUsers.Count; i++)
            {
                users[i].Id.Should().Be(expectedUsers[i].Id);
                users[i].Uuid.Should().Be(expectedUsers[i].Uuid);
                users[i].FirstName.Should().Be(expectedUsers[i].FirstName);
                users[i].LastName.Should().Be(expectedUsers[i].LastName);
                users[i].Email.Should().Be(expectedUsers[i].Email);
                users[i].Phone.Should().Be(expectedUsers[i].Phone);

                users[i].Role.Should().NotBeNull();
                users[i].Role.Label.Should().Be(expectedUsers[i].Role.Label);
                users[i].Role.MaxReservations.Should().Be(expectedUsers[i].Role.MaxReservations);
            }
        }

        private void ValidateAddress(Address expectedAddress, AddressDto address)
        {
            expectedAddress.Should().NotBeNull();
            expectedAddress.City.Should().NotBeNull();

            expectedAddress.CityId.Should().Be(address.CityId);
            expectedAddress.City.PostalCode.Should().Be(address.PostalCode);
            expectedAddress.City.Name.Should().Be(address.City);
            expectedAddress.Street.Should().Be(address.Street);
            expectedAddress.Number.Should().Be(address.Number);
        }
    }
}