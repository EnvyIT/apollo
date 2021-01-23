using Microsoft.AspNetCore.Authorization;

namespace Apollo.Api.Authorization
{
    public class RolesRequirement : IAuthorizationRequirement
    {
        public ApolloRoles[] Roles { get; }

        public RolesRequirement(ApolloRoles[] roles)
        {
            Roles = roles;
        }
    }
}