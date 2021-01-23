using System;
using Apollo.Persistence.Attributes.Attributes;
using Apollo.Persistence.Attributes.Base;

namespace Apollo.Persistence.Test.Entity.Mock
{
    [EntityTable("user")]
    public class UserMock : BaseEntity<UserMock>
    {
        [EntityColumn("uuid")]
        public string Uuid { get; set; }

        [EntityColumn("role")]
        public long RoleId { get; set; }

        [EntityColumnRef("role")]
        public RoleMock Role { get; set; }

        [EntityColumn("first_name")]
        public string FirstName { get; set; }

        [EntityColumn("last_name")]
        public string LastName { get; set; }

        [EntityColumn("email")]
        public string Email { get; set; }

        [EntityColumn("phone")]
        public string Phone { get; set; }

        [EntityColumn("address_id")]
        public long AddressId { get; set; }

        [EntityColumnRef("address")]
        public AddressMock Address { get; set; }

        public string Name => string.Empty == LastName ? FirstName : $"{FirstName} {LastName}";

        public override object Clone()
        {
            var clone = (UserMock)MemberwiseClone();
            clone.Address = (AddressMock)Address?.Clone();
            clone.Role = (RoleMock)Role?.Clone();
            return clone;
        }

        public override bool Equals(UserMock other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id && RoleId == other.RoleId && Uuid == other.Uuid &&
                   FirstName == other.FirstName && LastName == other.LastName &&
                   Email == other.Email && Phone == other.Phone && AddressId == other.AddressId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UserMock)obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, RoleId, Uuid, FirstName, LastName, Email, Phone, AddressId);
        }
    }
}
