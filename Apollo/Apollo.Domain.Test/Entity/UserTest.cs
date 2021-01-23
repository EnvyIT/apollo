using System;
using Apollo.Domain.Entity;
using FluentAssertions;

namespace Apollo.Domain.Test.Entity
{
    public class UserTest : EntityTest<User>
    {
        private readonly string _attributeTableName = "user";
        private readonly string _attributeColumnRoleId = "role";
        private readonly string _attributeColumnUuid = "uuid";
        private readonly string _attributeColumnFirstName = "first_name";
        private readonly string _attributeColumnLastName = "last_name";
        private readonly string _attributeColumnEmail = "email";
        private readonly string _attributeColumnPhone = "phone";
        private readonly string _attributeColumnAddressId = "address_id";
        private readonly string _attributeColumnRefRole = "role";
        private readonly string _attributeColumnRefAddress = "address";

        private readonly long _id = 1L;
        private readonly DateTime _rowVersion = DateTime.UtcNow;
        private readonly string _uuid = "e9f138e6-e1d4-4430-bb89-553ca8367f20";
        private readonly long _roleId = 10L;
        private readonly string _firstName = "First";
        private readonly string _lastName = "Last";
        private readonly string _email = "Email";
        private readonly string _phone = "123456789";
        private readonly long _addressId = 20L;
        private readonly Role _role = new Role { Id = 30L, RowVersion = DateTime.UtcNow, Label = "Label", MaxReservations = 15 };
        private readonly Address _address = new Address
        {
            Id = 40L,
            RowVersion = DateTime.UtcNow,
            CityId = 50L,
            Number = 26,
            Street = "Street"
        };

        private readonly long _cloneId = 11L;
        private readonly DateTime _cloneRowVersion = DateTime.UtcNow.AddMinutes(1);
        private readonly string _cloneUuid = "3f44fea0-2bd9-4ce7-998f-9492086f27cf";
        private readonly long _cloneRoleId = 11L;
        private readonly string _cloneFirstName = "Clone";
        private readonly string _cloneLastName = "Last clone";
        private readonly string _cloneEmail = "Email_Clone";
        private readonly string _clonePhone = "65547899";
        private readonly long _cloneAddressId = 21L;
        private readonly Role _cloneRole = new Role { Id = 31L, RowVersion = DateTime.UtcNow, Label = "Label 2", MaxReservations = 16 };
        private readonly Address _cloneAddress = new Address
        {
            Id = 41L,
            RowVersion = DateTime.UtcNow,
            CityId = 51L,
            Number = 27,
            Street = "New Street"
        };

        protected override void SetProperties(User value)
        {
            value.Id = _id;
            value.RowVersion = _rowVersion;
            value.Uuid = _uuid;
            value.RoleId = _roleId;
            value.FirstName = _firstName;
            value.LastName = _lastName;
            value.Email = _email;
            value.Phone = _phone;
            value.AddressId = _addressId;
            value.Role = _role;
            value.Address = _address;
        }

        protected override void SetCloneProperties(User value)
        {
            value.Id = _cloneId;
            value.RowVersion = _cloneRowVersion;
            value.Uuid = _cloneUuid;
            value.RoleId = _cloneRoleId;
            value.FirstName = _cloneFirstName;
            value.LastName = _cloneLastName;
            value.Email = _cloneEmail;
            value.Phone = _clonePhone;
            value.AddressId = _cloneAddressId;
            value.Role = _cloneRole;
            value.Address = _cloneAddress;
        }

        protected override void CheckProperties(User value)
        {
            value.Id.Should().Be(_id);
            value.RowVersion.Should().Be(_rowVersion);
            value.Uuid.Should().Be(_uuid);
            value.RoleId.Should().Be(_roleId);
            value.FirstName.Should().Be(_firstName);
            value.LastName.Should().Be(_lastName);
            value.Name.Should().Be($"{_firstName} {_lastName}");
            value.Email.Should().Be(_email);
            value.Phone.Should().Be(_phone);
            value.AddressId.Should().Be(_addressId);
            value.Role.Should().Be(_role);
            value.Address.Should().Be(_address);
        }

        protected override void CheckClonedProperties(User value)
        {
            value.Id.Should().Be(_cloneId);
            value.RowVersion.Should().Be(_cloneRowVersion);
            value.Uuid.Should().Be(_cloneUuid);
            value.RoleId.Should().Be(_cloneRoleId);
            value.FirstName.Should().Be(_cloneFirstName);
            value.LastName.Should().Be(_cloneLastName);
            value.Name.Should().Be($"{_cloneFirstName} {_cloneLastName}");
            value.Email.Should().Be(_cloneEmail);
            value.Phone.Should().Be(_clonePhone);
            value.AddressId.Should().Be(_cloneAddressId);
            value.Role.Should().Be(_cloneRole);
            value.Address.Should().Be(_cloneAddress);
        }

        protected override int CalculateHashCode()
        {
            return HashCode.Combine(_id, _roleId, _uuid, _firstName, _lastName, _email, _phone, _addressId);
        }

        protected override string GetAttributeTableName()
        {
            return _attributeTableName;
        }

        protected override void CheckAttributeColumns()
        {
            Attribute_Column_Name_Should(_ => _.RoleId, _attributeColumnRoleId);
            Attribute_Column_Name_Should(_ => _.Uuid, _attributeColumnUuid);
            Attribute_Column_Name_Should(_ => _.FirstName, _attributeColumnFirstName);
            Attribute_Column_Name_Should(_ => _.LastName, _attributeColumnLastName);
            Attribute_Column_Name_Should(_ => _.Email, _attributeColumnEmail);
            Attribute_Column_Name_Should(_ => _.Phone, _attributeColumnPhone);
            Attribute_Column_Name_Should(_ => _.AddressId, _attributeColumnAddressId);
            Attribute_ColumnRef_Name_Should(_ => _.Role, _attributeColumnRefRole);
            Attribute_ColumnRef_Name_Should(_ => _.Address, _attributeColumnRefAddress);
        }
    }
}