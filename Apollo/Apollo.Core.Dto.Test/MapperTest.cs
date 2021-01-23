using System;
using System.Collections.Generic;
using System.Linq;
using Apollo.Domain.Entity;
using FluentAssertions;
using NUnit.Framework;

namespace Apollo.Core.Dto.Test
{
    public class Tests
    {
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

        [Test]
        public void Test_Mapping_User()
        {
            Action action = () => Mapper.Map((User)null);
            action.Should().Throw<ArgumentNullException>();

            var user = (User)Users.First().Clone();
            user.Role = null;
            user.Address = null;
            var result = Mapper.Map(user);

            result.Id.Should().Be(user.Id);
            result.Uuid.Should().Be(user.Uuid);
            result.FirstName.Should().Be(user.FirstName);
            result.LastName.Should().Be(user.LastName);
            result.Phone.Should().Be(user.Phone);
            result.Email.Should().Be(user.Email);
            result.Role.Should().BeNull();
            result.Address.Should().BeNull();
        }

        [Test]
        public void Test_Mapping_User_WithReferences()
        {
            var user = Users.First();
            var result = Mapper.Map(user);

            result.Role.Should().NotBeNull();
            result.Address.Should().NotBeNull();
        }

        [Test]
        public void Test_Mapping_User_WithExplicitReferences()
        {
            var user = Users.First();

            Action action = () => Mapper.Map(null, user.Address, user.Role);
            action.Should().Throw<ArgumentNullException>();

            action = () => Mapper.Map(user, null, user.Role);
            action.Should().Throw<ArgumentNullException>();

            action = () => Mapper.Map(user, user.Address, null);
            action.Should().Throw<ArgumentNullException>();

            var result = Mapper.Map(user, user.Address, user.Role);

            result.Id.Should().Be(user.Id);
            result.Uuid.Should().Be(user.Uuid);
            result.FirstName.Should().Be(user.FirstName);
            result.LastName.Should().Be(user.LastName);
            result.Phone.Should().Be(user.Phone);
            result.Email.Should().Be(user.Email);

            result.Role.Should().NotBeNull();
            result.Role.Id.Should().Be(user.Role.Id);
            result.Role.Label.Should().Be(user.Role.Label);
            result.Role.MaxReservations.Should().Be(user.Role.MaxReservations);

            ValidateAddress(user.Address, result.Address);
        }

        [Test]
        public void Test_Mapping_Role()
        {
            Action action = () => Mapper.Map((Role)null);
            action.Should().Throw<ArgumentNullException>();

            var role = Roles.First();
            var result = Mapper.Map(role);

            result.Id.Should().Be(role.Id);
            result.Label.Should().Be(role.Label);
            result.MaxReservations.Should().Be(role.MaxReservations);
        }

        [Test]
        public void Test_Mapping_Address()
        {
            Action action = () => Mapper.Map((Address)null);
            action.Should().Throw<ArgumentNullException>();

            var address = Users.First().Address;
            var result = Mapper.Map(address);

            ValidateAddress(address, result);
        }

        [Test]
        public void Test_Mapping_Dto_To_Domain()
        {
            Action action = () => Mapper.Map((UserDto)null);
            action.Should().Throw<ArgumentNullException>();

            var user = Mapper.Map(Users.First());
            var result = Mapper.Map(user);
            ValidateUsers(new List<User> { result }, new List<UserDto> { user });

            user = Mapper.Map(Users.First());
            user.Address = null;
            result = Mapper.Map(user);
            ValidateUsers(new List<User> { result }, new List<UserDto> { user });

            user = Mapper.Map(Users.First());
            user.Role = null;
            result = Mapper.Map(user);
            result.Role.Should().BeNull();
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