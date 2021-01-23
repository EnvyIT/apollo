using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Apollo.Api.Authorization
{
    public class RolePolicyProvider : IAuthorizationPolicyProvider
    {
        private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

        public RolePolicyProvider(IOptions<AuthorizationOptions> options)
        {
            _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }


        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            var defaultValue = _fallbackPolicyProvider.GetPolicyAsync(policyName);

            if (!policyName.StartsWith(AuthorizeRoleAttribute.PolicyName, StringComparison.OrdinalIgnoreCase))
            {
                return defaultValue;
            }

            var policyTokens = policyName.Split(AuthorizeRoleAttribute.PolicyDelimiter,
                StringSplitOptions.RemoveEmptyEntries);
            if (policyTokens.Length != 2)
            {
                return defaultValue;
            }

            var policy = new AuthorizationPolicyBuilder("Bearer");
            var roles = policyTokens[1]
                .Split(AuthorizeRoleAttribute.RoleDelimiter, StringSplitOptions.RemoveEmptyEntries)
                .Select(Enum.Parse<ApolloRoles>)
                .ToArray();
            policy.AddRequirements(new RolesRequirement(roles));

            return Task.FromResult(policy.Build());
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() => _fallbackPolicyProvider.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync() => _fallbackPolicyProvider.GetFallbackPolicyAsync();
    }
}