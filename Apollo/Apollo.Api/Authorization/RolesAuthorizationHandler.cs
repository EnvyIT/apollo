using System;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Core.Interfaces;
using Apollo.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace Apollo.Api.Authorization
{
    public class RolesAuthorizationHandler : AuthorizationHandler<RolesRequirement>
    {
        private readonly ILogger<RolesAuthorizationHandler> _logger;
        private readonly IServiceFactory _serviceFactory;

        public RolesAuthorizationHandler(ILogger<RolesAuthorizationHandler> logger, IServiceFactory serviceFactory)
        {
            _logger = logger;
            _serviceFactory = serviceFactory;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            RolesRequirement requirement)
        {
            var uuid = JwtHelper.GetUuidFromClaims(context.User.Claims);
            if (uuid == null)
            {
                _logger.LogWarning("No UUID given!");
                return;
            }

            try
            {
                var user = await _serviceFactory.CreateUserService().GetUserWithAddressByUuidAsync(uuid);
                if (requirement.Roles.Contains(ApolloRoles.All) || requirement.Roles.Contains(Enum.Parse<ApolloRoles>(user.Role.Label)))
                {
                    _logger.LogInformation($"UUID {uuid} authorized.");
                    context.Succeed(requirement);
                }
            }
            catch (Exception e)
            {
                _logger.LogError("Validate UUID failed!", e);
            }
        }
    }
}