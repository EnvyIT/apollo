using System.Linq;
using Microsoft.AspNetCore.Authorization;
using static System.String;

namespace Apollo.Api.Authorization
{
    public class AuthorizeRoleAttribute : AuthorizeAttribute
    {
        public const string PolicyName = "RolesPolicy";
        public const string PolicyDelimiter = "$";
        public const string RoleDelimiter = "|";

        private ApolloRoles[] _roles;

        public AuthorizeRoleAttribute(params ApolloRoles[] roles) => Roles = roles;

        public ApolloRoles[] Roles
        {
            get => _roles;
            set
            {
                _roles = value;
                Policy = $"{PolicyName}{PolicyDelimiter}{Join(RoleDelimiter, value.Select(role => role.ToString()))}";
            }
        }
    }
}