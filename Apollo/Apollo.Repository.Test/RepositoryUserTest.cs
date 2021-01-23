using System;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Domain.Entity;
using Apollo.Persistence.Exception;
using Apollo.Repository.Interfaces;
using Apollo.Util;
using FluentAssertions;
using FluentAssertions.Common;
using NUnit.Framework;

namespace Apollo.Repository.Test
{
    public class RepositoryUserTest
    {
        private IRepositoryUser _repositoryUser;
        private RepositoryHelper _repositoryHelper;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _repositoryHelper = new RepositoryHelper();
            _repositoryHelper.FillTypes.AddAll(new[]
            {
                EntityType.User, EntityType.Address, EntityType.City, EntityType.Role
            });
            _repositoryUser = _repositoryHelper.RepositoryFactory.CreateRepositoryUser();
        }

        [SetUp]
        public async Task Setup()
        {
            await _repositoryHelper.SeedDatabase();
        }

        [TearDown]
        public async Task TearDown()
        {
            await _repositoryHelper.TearDown();
        }

        [Test]
        public async Task Test_GetById_WithInvalidId_ShouldThrow()
        {
            Func<Task> execution = async () => await _repositoryUser.GetByIdAsync(500L);
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_GetById()
        {
            var expected = _repositoryHelper.Users.First();
            var result = await _repositoryUser.GetByIdAsync(expected.Id);
            result.Should().Be(expected);
        }

        [Test]
        public async Task Test_GetRoles()
        {
            var result = (await _repositoryUser.GetRolesAsync()).OrderBy(movie => movie.Id).ToList();

            result.Should().HaveCount(_repositoryHelper.Roles.Count());
            result.Should().IsSameOrEqualTo(_repositoryHelper.Roles.OrderBy(_ => _.Id));
        }

        [Test]
        public async Task Test_GetRoles_WithoutDeleted()
        {
            var roleId1 = _repositoryHelper.Roles.ElementAt(0).Id;
            var roleId2 = _repositoryHelper.Roles.ElementAt(1).Id;

            await _repositoryUser.DeleteUserAsync(_repositoryHelper.Users.ElementAt(0).Id);
            await _repositoryUser.DeleteUserAsync(_repositoryHelper.Users.ElementAt(1).Id);
            await _repositoryUser.DeleteUserAsync(_repositoryHelper.Users.ElementAt(2).Id);
            await _repositoryUser.DeleteUserAsync(_repositoryHelper.Users.ElementAt(3).Id);
            await _repositoryUser.DeleteRoleAsync(roleId1);
            await _repositoryUser.DeleteRoleAsync(roleId2);

            var result = (await _repositoryUser.GetRolesAsync()).OrderBy(_ => _.Id).ToList();

            result.Should().HaveCount(_repositoryHelper.Roles.Count() - 2);
            result.Select(_ => _.Id).Should().NotContain(roleId1);
            result.Select(_ => _.Id).Should().NotContain(roleId2);
        }

        [Test]
        public async Task Test_GetUsers()
        {
            var result = (await _repositoryUser.GetUsersAsync()).OrderBy(_ => _.Id).ToList();

            result.Should().HaveCount(_repositoryHelper.Users.Count());
            result.Should().IsSameOrEqualTo(_repositoryHelper.Users.OrderBy(_ => _.Id));
        }

        [Test]
        public async Task Test_GetUsers_WithoutDeleted()
        {
            var deletedId1 = _repositoryHelper.Users.ElementAt(1).Id;
            var deletedId2 = _repositoryHelper.Users.ElementAt(4).Id;

            await _repositoryUser.DeleteUserAsync(deletedId1);
            await _repositoryUser.DeleteUserAsync(deletedId2);
            var result = (await _repositoryUser.GetUsersAsync()).OrderBy(_ => _.Id).ToList();

            result.Should().HaveCount(_repositoryHelper.Users.Count() - 2);
            result.Select(_ => _.Id).Should().NotContain(deletedId1);
            result.Select(_ => _.Id).Should().NotContain(deletedId2);
        }

        [Test]
        public async Task Test_GetUserWithAllReferencesByUuid_InvalidUuid_ShouldThrow()
        {
            Func<Task> execution = async () => await _repositoryUser.GetUserWithAllReferencesByUuidAsync("invalid");
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_GetUserWithAllReferencesByUuid()
        {
            var user = _repositoryHelper.Users.First();
            var result = await _repositoryUser.GetUserWithAllReferencesByUuidAsync(user.Uuid);

            result.Should().NotBeNull();
            result.Should().Be(user);

            result.Role.Should().NotBeNull();
            result.Role.Should().Be(_repositoryHelper.Roles.First(r => r.Id == user.RoleId));

            result.Address.Should().NotBeNull();
            result.Address.Should().Be(_repositoryHelper.Addresses.First(a => a.Id == user.AddressId));

            result.Address.City.Should().NotBeNull();
            result.Address.City.Should().Be(_repositoryHelper.Cities.First(c =>
                c.Id == _repositoryHelper.Addresses.First(a => a.Id == user.AddressId).CityId));
        }

        [Test]
        public async Task Test_GetUserWithAllReferencesById()
        {
            var user = _repositoryHelper.Users.First();
            var result = await _repositoryUser.GetUserWithAllReferencesByIdAsync(user.Id);

            result.Should().NotBeNull();
            result.Should().Be(user);

            result.Role.Should().NotBeNull();
            result.Role.Should().Be(_repositoryHelper.Roles.First(r => r.Id == user.RoleId));

            result.Address.Should().NotBeNull();
            result.Address.Should().Be(_repositoryHelper.Addresses.First(a => a.Id == user.AddressId));

            result.Address.City.Should().NotBeNull();
            result.Address.City.Should().Be(_repositoryHelper.Cities.First(c =>
                c.Id == _repositoryHelper.Addresses.First(a => a.Id == user.AddressId).CityId));
        }

        [Test]
        public async Task Test_GetDeletedUsers()
        {
            var users = new[]
            {
                _repositoryHelper.Users.ElementAt(1).Id,
                _repositoryHelper.Users.ElementAt(4).Id,
                _repositoryHelper.Users.ElementAt(8).Id
            };
            foreach (var user in users)
            {
                await _repositoryUser.DeleteUserAsync(user);
            }

            var result = (await _repositoryUser.GetDeletedUsersAsync()).OrderBy(_ => _.Id).ToList();

            result.Should().HaveCount(users.Length);
            result.Should().IsSameOrEqualTo(users.OrderBy(id => id));
        }

        [Test]
        public async Task Test_GetUsersWithRoleByRole_SingleRole_With_Invalid_Id_ShouldThrow()
        {
            Func<Task> execution = async () => await _repositoryUser.GetUsersWithRoleByRoleAsync(500L);
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_GetUsersWithRoleByRole_Without_Roles_ShouldThrow()
        {
            var userId = _repositoryHelper.Users.First().Id;
            await _repositoryUser.DeleteUserAsync(userId);

            Func<Task> execution = async () => await _repositoryUser.GetUserAddressWithCityAsync(userId);
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_GetUsersWithRoleByRole_SingleRole()
        {
            var roleId = 2L;
            var result = (await _repositoryUser.GetUsersWithRoleByRoleAsync(roleId)).OrderBy(_ => _.Id).ToList();

            var expected = _repositoryHelper.Users.Where(_ => _.RoleId == roleId).ToList();

            result.Should().HaveCount(expected.Count());
            for (int i = (int) roleId * expected.Count; i <= result.Count; i++)
            {
                var user = result[i - 1];
                var roleNumber = (i - 1) / 2 + 1;
                user.Name.Should().Be($"First {i} Last {i}");
                user.AddressId.Should().Be(i);
                user.Email.Should().Be($"mail_{i}@fh-ooe.at");
                user.Phone.Should().Be($"0664{string.Concat(Enumerable.Repeat(i - 1, 6))}");
                user.RoleId.Should().Be(roleNumber);

                user.Role.Should().NotBeNull();
                user.Role.Label.Should().Be($"Role {roleNumber}");
                user.Role.MaxReservations.Should().Be(roleNumber * 10);
            }
        }

        [Test]
        public async Task Test_GetUsersWithRoleByRole_With_Invalid_Id_ShouldThrow()
        {
            Func<Task> execution = async () => await _repositoryUser.GetUsersWithRoleByRoleAsync(new[] {1L, 500L});
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_GetUsersWithRoleByRole()
        {
            var rolesIds = new[]
            {
                _repositoryHelper.Roles.ElementAt(0).Id,
                _repositoryHelper.Roles.ElementAt(1).Id,
                _repositoryHelper.Roles.ElementAt(2).Id
            };
            var result = (await _repositoryUser.GetUsersWithRoleByRoleAsync(rolesIds)).OrderBy(_ => _.Id).ToList();

            var expected = _repositoryHelper.Users.Where(_ => rolesIds.Contains(_.RoleId));

            result.Should().HaveCount(expected.Count());
            for (int i = 1; i <= result.Count; i++)
            {
                var user = result[i - 1];
                var role = _repositoryHelper.Roles.Single(_ => _.Id == user.RoleId);
                user.Name.Should().Be($"First {i} Last {i}");
                user.AddressId.Should().Be(_repositoryHelper.Addresses.Single(_ => _.Id == user.AddressId).Id);
                user.Email.Should().Be($"mail_{i}@fh-ooe.at");
                user.Phone.Should().Be($"0664{string.Concat(Enumerable.Repeat(i - 1, 6))}");
                user.RoleId.Should().Be(role.Id);

                user.Role.Should().NotBeNull();
                user.Role.Label.Should().Be(role.Label);
                user.Role.MaxReservations.Should().Be(role.MaxReservations);
            }
        }

        [Test]
        public async Task Test_GetUserWithRoleByCity_With_Invalid_Id_ShouldThrow()
        {
            Func<Task> execution = async () => await _repositoryUser.GetUsersWithRoleByCityAsync(500000L);
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_GetUserWithRoleByCity()
        {
            var cityId = _repositoryHelper.Cities.First().Id;
            var result = (await _repositoryUser.GetUsersWithRoleByCityAsync(cityId)).OrderBy(_ => _.Id).ToList();

            var addressIds = _repositoryHelper.Addresses.Where(_ => _.CityId == cityId).Select(_ => _.Id).Distinct();
            var expected = _repositoryHelper.Users.Where(_ => addressIds.Contains(_.AddressId)).ToList();

            result.Should().HaveCount(expected.Count);
            for (int i = 1; i <= result.Count; i++)
            {
                var user = result[i - 1];
                var role = _repositoryHelper.Roles.Single(_ => _.Id == user.RoleId);
                user.Name.Should().Be($"First {i} Last {i}");
                user.AddressId.Should().Be(_repositoryHelper.Addresses.Single(_ => _.Id == user.AddressId).Id);
                user.Email.Should().Be($"mail_{i}@fh-ooe.at");
                user.Phone.Should().Be($"0664{string.Concat(Enumerable.Repeat(i - 1, 6))}");
                user.RoleId.Should().Be(role.Id);

                user.Role.Should().NotBeNull();
                user.Role.Label.Should().Be(role.Label);
                user.Role.MaxReservations.Should().Be(role.MaxReservations);
            }
        }

        [Test]
        public async Task Test_GetUserWithRoleByCity_Deleted()
        {
            var deletedUser = _repositoryHelper.Users.Last();
            var deletedCityId = _repositoryHelper.Cities
                .Single(c => c.Id == _repositoryHelper.Addresses
                    .Single(a => a.Id == deletedUser.AddressId).CityId).Id;
            var cityId = _repositoryHelper.Cities.First().Id;

            await _repositoryUser.DeleteUserAsync(deletedUser.Id);

            var result = (await _repositoryUser.GetUsersWithRoleByCityAsync(cityId)).OrderBy(_ => _.Id).ToList();

            var addressIds = _repositoryHelper.Addresses.Where(_ => _.CityId == cityId).Select(_ => _.Id).Distinct();
            var expected = _repositoryHelper.Users.Where(_ => _.Id != deletedCityId && addressIds.Contains(_.AddressId))
                .ToList();

            result.Should().HaveCount(expected.Count);
            for (int i = 1; i < result.Count; i++)
            {
                var user = result[i - 1];
                var role = _repositoryHelper.Roles.Single(_ => _.Id == user.RoleId);
                user.Name.Should().Be($"First {i} Last {i}");
                user.AddressId.Should().Be(_repositoryHelper.Addresses.Single(_ => _.Id == user.AddressId).Id);
                user.Email.Should().Be($"mail_{i}@fh-ooe.at");
                user.Phone.Should().Be($"0664{string.Concat(Enumerable.Repeat(i - 1, 6))}");
                user.RoleId.Should().Be(role.Id);

                user.Role.Should().NotBeNull();
                user.Role.Label.Should().Be(role.Label);
                user.Role.MaxReservations.Should().Be(role.MaxReservations);
            }
        }

        [Test]
        public async Task Test_GetUserAddressWithCity_With_Invalid_Id_ShouldThrow()
        {
            Func<Task> execution = async () => await _repositoryUser.GetUserAddressWithCityAsync(500000L);
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_GetUserAddressWithCity_Deleted_ShouldThrow()
        {
            var userId = _repositoryHelper.Users.First().Id;
            await _repositoryUser.DeleteUserAsync(userId);

            Func<Task> execution = async () => await _repositoryUser.GetUserAddressWithCityAsync(userId);
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_GetUserAddressWithCity()
        {
            var user = _repositoryHelper.Users.First();
            var address = _repositoryHelper.Addresses.Single(_ => _.Id == user.AddressId);
            var city = _repositoryHelper.Cities.Single(_ => _.Id == address.CityId);

            var result = await _repositoryUser.GetUserAddressWithCityAsync(user.Id);

            result.Street.Should().Be(address.Street);
            result.Number.Should().Be(address.Number);
            result.City.Should().NotBeNull();
            result.City.PostalCode.Should().Be(city.PostalCode);
            result.City.Name.Should().Be(city.Name);
        }


        [Test]
        public async Task Test_GetCities()
        {
            var cityId = _repositoryHelper.Cities.First().Id;
            await _repositoryUser.DeleteUserAsync(_repositoryHelper.Users.ElementAt(0).Id);
            await _repositoryUser.DeleteUserAsync(_repositoryHelper.Users.ElementAt(1).Id);
            await _repositoryUser.DeleteAddressAsync(_repositoryHelper.Addresses.ElementAt(0).Id);
            await _repositoryUser.DeleteAddressAsync(_repositoryHelper.Addresses.ElementAt(1).Id);
            await _repositoryUser.DeleteCityAsync(cityId);

            var result = (await _repositoryUser.GetCitiesAsync()).OrderBy(_ => _.Id).ToList();
            var expected = _repositoryHelper.Cities.Where(_ => _.Id != cityId).OrderBy(_ => _.Id).ToList();

            result.Should().HaveCount(expected.Count);
            result.Should().IsSameOrEqualTo(expected);
        }

        [Test]
        public async Task Test_UserExist_InvalidId_Should_ReturnFalse()
        {
            var result = await _repositoryUser.UserExistAsync(500L);
            result.Should().BeFalse();
        }

        [Test]
        public async Task Test_UserExist_ValidId_Should_ReturnTrue()
        {
            var result = await _repositoryUser.UserExistAsync(_repositoryHelper.Users.First().Id);
            result.Should().BeTrue();
        }

        [Test]
        public async Task Test_CityExist_Invalid_PostalCode()
        {
            var result = await _repositoryUser.CityExistAsync("1234", "City 1");
            result.Exist.Should().BeFalse();
        }

        [Test]
        public async Task Test_CityExist_Invalid_City()
        {
            var result = await _repositoryUser.CityExistAsync("1111", "Hagenberg");
            result.Exist.Should().BeFalse();
        }

        [Test]
        public async Task Test_CityExist_Invalid_Combination()
        {
            var result = await _repositoryUser.CityExistAsync("1111", "City 2");
            result.Exist.Should().BeFalse();
        }

        [Test]
        public async Task Test_CityExist_Deleted()
        {
            var city = _repositoryHelper.Cities.First();

            await _repositoryUser.DeleteUserAsync(_repositoryHelper.Users.ElementAt(0).Id);
            await _repositoryUser.DeleteUserAsync(_repositoryHelper.Users.ElementAt(1).Id);
            await _repositoryUser.DeleteAddressAsync(_repositoryHelper.Addresses.ElementAt(0).Id);
            await _repositoryUser.DeleteAddressAsync(_repositoryHelper.Addresses.ElementAt(1).Id);
            await _repositoryUser.DeleteCityAsync(city.Id);

            var result = await _repositoryUser.CityExistAsync(city.Name, city.Name);
            result.Exist.Should().BeFalse();
        }

        [Test]
        public async Task Test_CityExist_Valid()
        {
            var city = _repositoryHelper.Cities.First();

            var result = await _repositoryUser.CityExistAsync(city.PostalCode, city.Name);

            result.Exist.Should().BeTrue();
            result.Id.Should().Be(city.Id);
        }

        [Test]
        public async Task Test_AddUser_Null_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryUser.AddUserAsync(null);
            await execution.Should().ThrowAsync<ArgumentNullException>();
        }

        [Test]
        public async Task Test_AddUser_Duplicate_ShouldThrow()
        {
            Func<Task> execution = async () => await _repositoryUser.AddUserAsync(
                new User {Id = _repositoryHelper.Users.First().Id});
            await execution.Should().ThrowAsync<DuplicateEntryException>();
        }

        [Test]
        public async Task Test_AddUser_With_Invalid_AddressId_ShouldThrow()
        {
            Func<Task> execution = async () =>
                await _repositoryUser.AddUserAsync(new User {Id = 100000L, AddressId = 500000L});
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_AddUser_With_Invalid_AddressObject_ShouldThrow()
        {
            Func<Task> execution = async () =>
                await _repositoryUser.AddUserAsync(new User {Id = 100000L, Address = new Address {Id = 500000L}});
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_AddUser_Without_Address_ShouldThrow()
        {
            Func<Task> execution = async () => await _repositoryUser.AddUserAsync(new User {Id = 100000L});
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_AddUser()
        {
            var user = new User
            {
                Uuid = "5ebf8624-32f2-403d-81fc-da70df50cd3d",
                AddressId = _repositoryHelper.Addresses.Last().Id,
                Email = "new_user@gmx.at",
                FirstName = "New",
                LastName = "User",
                Phone = "066412345678",
                RoleId = _repositoryHelper.Roles.First().Id
            };
            var result = await _repositoryUser.AddUserAsync(user);
            result.Should().BeGreaterThan(0L);
            user.Id = result;

            var userResult = (await _repositoryUser.GetUsersAsync()).FirstOrDefault(_ => _.Id == result);

            userResult.Should().NotBeNull();
            userResult.Should().Be(user);
        }

        [Test]
        public async Task Test_AddAddress_Null_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryUser.AddAddressAsync(null);
            await execution.Should().ThrowAsync<ArgumentNullException>();
        }

        [Test]
        public async Task Test_AddAddress_Duplicate_ShouldThrow()
        {
            Func<Task> execution = async () => await _repositoryUser.AddAddressAsync(
                new Address {Id = _repositoryHelper.Addresses.First().Id}
            );
            await execution.Should().ThrowAsync<DuplicateEntryException>();
        }

        [Test]
        public async Task Test_AddAddress_With_Invalid_CityId_ShouldThrow()
        {
            Func<Task> execution = async () =>
                await _repositoryUser.AddAddressAsync(new Address {Id = 100000L, CityId = 500000L});
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_AddAddress_With_Invalid_CityObject_ShouldThrow()
        {
            Func<Task> execution = async () =>
                await _repositoryUser.AddAddressAsync(new Address {Id = 100000L, City = new City {Id = 500000L}});
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_AddAddress_Without_City_ShouldThrow()
        {
            Func<Task> execution = async () => await _repositoryUser.AddAddressAsync(new Address {Id = 100000L});
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_AddAddress()
        {
            var address = new Address
            {
                Street = "Campusstrasse",
                Number = 42,
                CityId = _repositoryHelper.Cities.First().Id
            };
            var result = await _repositoryUser.AddAddressAsync(address);
            result.Should().BeGreaterThan(0L);
            address.Id = result;

            var addressResult = await _repositoryHelper.FluentEntity
                .SelectAll<Address>()
                .Where(_ => _.Id)
                .Equal(result).QuerySingleAsync();

            addressResult.Should().NotBeNull();
            addressResult.Should().Be(address);
        }

        [Test]
        public async Task Test_AddCity_Null_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryUser.AddCityAsync(null);
            await execution.Should().ThrowAsync<ArgumentNullException>();
        }

        [Test]
        public async Task Test_AddCity_Duplicate_Id_ShouldThrow()
        {
            Func<Task> execution = async () => await _repositoryUser.AddCityAsync(
                new City {Id = _repositoryHelper.Cities.First().Id});
            await execution.Should().ThrowAsync<DuplicateEntryException>();
        }

        [Test]
        public async Task Test_AddCity_Duplicate_PostalCodeName_ShouldThrow()
        {
            var city = _repositoryHelper.Cities.First();
            Func<Task> execution = async () =>
                await _repositoryUser.AddCityAsync(new City {PostalCode = city.PostalCode, Name = city.Name});
            await execution.Should().ThrowAsync<DuplicateEntryException>();
        }

        [Test]
        public async Task Test_AddCity()
        {
            var city = new City
            {
                PostalCode = "4600",
                Name = "Wels"
            };
            var result = await _repositoryUser.AddCityAsync(city);
            result.Should().BeGreaterThan(0L);
            city.Id = result;

            var cityResult = await _repositoryHelper.FluentEntity
                .SelectAll<City>()
                .Where(_ => _.Id)
                .Equal(result).QuerySingleAsync();

            cityResult.Should().NotBeNull();
            cityResult.Should().Be(city);
        }

        [Test]
        public async Task Test_AddRole_Null_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryUser.AddRoleAsync(null);
            await execution.Should().ThrowAsync<ArgumentNullException>();
        }

        [Test]
        public async Task Test_AddRole_Duplicate_Id_ShouldThrow()
        {
            Func<Task> execution = async () => await _repositoryUser.AddRoleAsync(new Role {Id = 1L});
            await execution.Should().ThrowAsync<DuplicateEntryException>();
        }

        [Test]
        public async Task Test_AddRole_Duplicate_Name_ShouldThrow()
        {
            var role = _repositoryHelper.Roles.First();
            Func<Task> execution = async () => await _repositoryUser.AddRoleAsync(new Role {Label = role.Label});
            await execution.Should().ThrowAsync<DuplicateEntryException>();
        }

        [Test]
        public async Task Test_AddRole()
        {
            var role = new Role
            {
                Label = "The boss",
                MaxReservations = 100
            };
            var result = await _repositoryUser.AddRoleAsync(role);
            result.Should().BeGreaterThan(0L);
            role.Id = result;

            var roleResult = (await _repositoryUser.GetRolesAsync()).First(_ => _.Id == result);

            roleResult.Should().NotBeNull();
            roleResult.Should().Be(role);
        }

        [Test]
        public async Task Test_DeleteUser_With_InvalidId_ShouldThrow()
        {
            Func<Task> execution = async () => await _repositoryUser.DeleteUserAsync(100L);
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_DeleteUser()
        {
            var user = _repositoryHelper.Users.First();

            var result = await _repositoryUser.DeleteUserAsync(user.Id);
            result.Should().Be(1);

            var users = (await _repositoryUser.GetUsersAsync()).Select(_ => _.Id).ToList();
            var addresses =
                (await _repositoryHelper.FluentEntity.SelectAll<Address>().WhereActive().QueryAsync())
                .Select(_ => _.Id);
            var cities = (await _repositoryUser.GetCitiesAsync()).Select(_ => _.Id);

            users.Should().NotContain(user.Id);
            users.Should().HaveCount(_repositoryHelper.Users.Count() - 1);
            addresses.Should().NotContain(user.AddressId);
            cities.Should().Contain(_repositoryHelper.Cities.First().Id);
        }

        [Test]
        public async Task Test_DeleteAddress_With_InvalidId_ShouldThrow()
        {
            Func<Task> execution = async () => await _repositoryUser.DeleteAddressAsync(1000000L);
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_DeleteAddress_With_UserReference_ShouldThrow()
        {
            Func<Task> execution = async () =>
                await _repositoryUser.DeleteAddressAsync(_repositoryHelper.Addresses.First().Id);
            await execution.Should().ThrowAsync<EntityReferenceException>();
        }

        [Test]
        public async Task Test_DeleteAddress()
        {
            var addressId = _repositoryHelper.Addresses.First().Id;
            await _repositoryHelper.FluentEntity.Delete<User>().Where(_ => _.Id)
                .Equal(_repositoryHelper.Users.First().Id).ExecuteAsync();

            var result = await _repositoryUser.DeleteAddressAsync(addressId);
            result.Should().Be(1);

            var addresses = (await _repositoryHelper.FluentEntity
                    .SelectAll<Address>()
                    .WhereActive()
                    .QueryAsync())
                .Select(_ => _.Id)
                .ToList();
            var cities = (await _repositoryUser.GetCitiesAsync()).Select(_ => _.Id);

            addresses.Should().NotContain(addressId);
            addresses.Should().HaveCount(_repositoryHelper.Addresses.Count() - 1);
            cities.Should().Contain(_repositoryHelper.Cities.First().Id);
        }

        [Test]
        public async Task Test_DeleteCity_With_InvalidId_ShouldThrow()
        {
            Func<Task> execution = async () => await _repositoryUser.DeleteCityAsync(100L);
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_DeleteCity_With_AddressReference_ShouldThrow()
        {
            Func<Task> execution = async () =>
                await _repositoryUser.DeleteCityAsync(_repositoryHelper.Cities.First().Id);
            await execution.Should().ThrowAsync<EntityReferenceException>();
        }

        [Test]
        public async Task Test_DeleteCity()
        {
            var id = _repositoryHelper.Cities.First().Id;
            await _repositoryHelper.FluentEntity.Delete<User>().Where(_ => _.Id)
                .Equal(_repositoryHelper.Users.ElementAt(0).Id).ExecuteAsync();
            await _repositoryHelper.FluentEntity.Delete<User>().Where(_ => _.Id)
                .Equal(_repositoryHelper.Users.ElementAt(1).Id).ExecuteAsync();
            await _repositoryHelper.FluentEntity.Delete<Address>().Where(_ => _.Id)
                .Equal(_repositoryHelper.Addresses.ElementAt(0).Id).ExecuteAsync();
            await _repositoryHelper.FluentEntity.Delete<Address>().Where(_ => _.Id)
                .Equal(_repositoryHelper.Addresses.ElementAt(1).Id).ExecuteAsync();

            var result = await _repositoryUser.DeleteCityAsync(id);
            result.Should().Be(1);

            var cities = (await _repositoryUser.GetCitiesAsync()).Select(_ => _.Id).ToList();
            cities.Should().NotContain(id);
            cities.Should().HaveCount(_repositoryHelper.Cities.Count() - 1);
        }

        [Test]
        public async Task Test_DeleteRole_With_InvalidId_ShouldThrow()
        {
            Func<Task> execution = async () => await _repositoryUser.DeleteRoleAsync(100L);
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_DeleteRole_With_UserReference_ShouldThrow()
        {
            Func<Task> execution = async () => await _repositoryUser.DeleteRoleAsync(1L);
            await execution.Should().ThrowAsync<EntityReferenceException>();
        }

        [Test]
        public async Task Test_DeleteRole()
        {
            var id = _repositoryHelper.Roles.First().Id;
            await _repositoryHelper.FluentEntity.Delete<User>().Where(_ => _.Id)
                .Equal(_repositoryHelper.Users.ElementAt(0).Id).ExecuteAsync();
            await _repositoryHelper.FluentEntity.Delete<User>().Where(_ => _.Id)
                .Equal(_repositoryHelper.Users.ElementAt(1).Id).ExecuteAsync();

            var result = await _repositoryUser.DeleteRoleAsync(id);
            result.Should().Be(1);

            var roles = (await _repositoryUser.GetRolesAsync()).Select(_ => _.Id).ToList();
            roles.Should().NotContain(id);
            roles.Should().HaveCount(_repositoryHelper.Roles.Count() - 1);
        }

        [Test]
        public async Task Test_UpdateUser_InvalidId_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryUser.UpdateUserAsync(new User {Id = 500L});
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_UpdateUser_InvalidAddressId_Should_Throw()
        {
            Func<Task> execution = async () =>
                await _repositoryUser.UpdateUserAsync(new User {Id = 1L, AddressId = 500L});
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_UpdateUser_InvalidAddressObject_Should_Throw()
        {
            Func<Task> execution = async () =>
                await _repositoryUser.UpdateUserAsync(new User {Id = 1L, Address = new Address {Id = 500L}});
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_UpdateUser_NoAddress_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryUser.UpdateUserAsync(new User {Id = 1L});
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_UpdateUser_Null_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryUser.UpdateUserAsync(null);
            await execution.Should().ThrowAsync<ArgumentNullException>();
        }

        [Test]
        public async Task Test_UpdateUser()
        {
            var userId = _repositoryHelper.Users.First().Id;
            var user = (await _repositoryUser.GetUsersAsync()).First(_ => _.Id == userId);
            user.Email = "new@gmail.com";
            user.FirstName = "New";
            user.LastName = "Name";
            user.Phone = "1523456";

            var result = await _repositoryUser.UpdateUserAsync(user);
            result.Should().Be(1);

            var resultUsers = (await _repositoryUser.GetUsersAsync()).Where(_ => _.Id == userId).ToList();
            resultUsers.Select(_ => _.Id).Should().Contain(userId);
            resultUsers.First(_ => _.Id == userId).Equals(user).Should().BeTrue();
        }

        [Test]
        public async Task Test_UpdateAddress_InvalidId_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryUser.UpdateAddressAsync(new Address {Id = 500L});
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_UpdateAddress_InvalidCityId_Should_Throw()
        {
            Func<Task> execution = async () =>
                await _repositoryUser.UpdateAddressAsync(new Address {Id = 1L, CityId = 500L});
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_UpdateAddress_InvalidAddressObject_Should_Throw()
        {
            Func<Task> execution = async () =>
                await _repositoryUser.UpdateAddressAsync(new Address {Id = 1L, City = new City {Id = 500L}});
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_UpdateAddress_NoAddress_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryUser.UpdateAddressAsync(new Address {Id = 1L});
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_UpdateAddress_Null_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryUser.UpdateAddressAsync(null);
            await execution.Should().ThrowAsync<ArgumentNullException>();
        }

        [Test]
        public async Task Test_UpdateAddress()
        {
            var addressId = _repositoryHelper.Addresses.First().Id;
            var query = _repositoryHelper.FluentEntity
                .SelectAll<Address>(false)
                .WhereActive()
                .And(_ => _.Id)
                .Equal(addressId);
            var address = await query.QuerySingleAsync();
            address.Street = "Campus";
            address.Number = 150;

            var result = await _repositoryUser.UpdateAddressAsync(address);
            result.Should().Be(1);

            var resultAddresses = await query.QuerySingleAsync();
            resultAddresses.Should().Be(address);
        }

        [Test]
        public async Task Test_UpdateCity_InvalidId_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryUser.UpdateCityAsync(new City {Id = 500000L});
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_UpdateCity_Null_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryUser.UpdateCityAsync(null);
            await execution.Should().ThrowAsync<ArgumentNullException>();
        }

        [Test]
        public async Task Test_UpdateCity()
        {
            var cityId = _repositoryHelper.Cities.First().Id;
            var city = (await _repositoryUser.GetCitiesAsync()).First(_ => _.Id == cityId);
            city.PostalCode = "9876";
            city.Name = "Hagenberg";

            var result = await _repositoryUser.UpdateCityAsync(city);
            result.Should().Be(1);

            var resultCity = (await _repositoryUser.GetCitiesAsync()).Where(_ => _.Id == cityId).ToList();
            resultCity.Select(_ => _.Id).Should().Contain(cityId);
            resultCity.First(_ => _.Id == cityId).Equals(city).Should().BeTrue();
        }

        [Test]
        public async Task Test_UpdateRole_InvalidId_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryUser.UpdateRoleAsync(new Role {Id = 500L});
            await execution.Should().ThrowAsync<InvalidEntityIdException>();
        }

        [Test]
        public async Task Test_UpdateRole_Null_Should_Throw()
        {
            Func<Task> execution = async () => await _repositoryUser.UpdateRoleAsync(null);
            await execution.Should().ThrowAsync<ArgumentNullException>();
        }

        [Test]
        public async Task Test_UpdateRole()
        {
            var role = (await _repositoryUser.GetRolesAsync()).First(_ => _.Id == 1L);
            role.Label = "Best customer";
            role.MaxReservations = 50;

            var result = await _repositoryUser.UpdateRoleAsync(role);
            result.Should().Be(1);

            var resultRole = (await _repositoryUser.GetRolesAsync()).Where(_ => _.Id == 1L).ToList();
            resultRole.Select(_ => _.Id).Should().Contain(1L);
            resultRole.First(_ => _.Id == 1L).Equals(role).Should().BeTrue();
        }
    }
}