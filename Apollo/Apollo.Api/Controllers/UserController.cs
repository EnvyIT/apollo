using System;
using System.Threading.Tasks;
using Apollo.Api.Authorization;
using Apollo.Api.Controllers.Base;
using Apollo.Core.Dto;
using Apollo.Core.Interfaces;
using Apollo.Util;
using Apollo.Util.Logger;
using Microsoft.AspNetCore.Mvc;

namespace Apollo.Api.Controllers
{
    public class UserController : ApiControllerBase
    {
        private static readonly IApolloLogger<UserController> Logger = LoggerFactory.CreateLogger<UserController>();
        private readonly IServiceFactory _service;

        public UserController(IServiceFactory service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// Gets the specific role from a given JWT token.
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /v1/user/role
        ///
        /// </remarks>
        /// <returns>Return the role for the given JWT-token.</returns>
        /// <response code="200">Returns the role for the given JWT-token.</response>
        /// <response code="400">If role JWT is not valid.</response>
        [HttpGet("role")]
        [AuthorizeRole(ApolloRoles.All)]
        public async Task<ActionResult<RoleDto>> GetRole()
        {
            var uuid = JwtHelper.GetUuidFromClaims(HttpContext.User.Claims);
            if (uuid == null)
            {
                Logger.Here().Error("Request with invalid JWT");
                return BadRequestResponse("JWT invalid");
            }
            var user = await _service.CreateUserService().GetUserWithAddressByUuidAsync(uuid);
            Logger.Here().Info("{Role} extracted from claims for {user}", user.Role, user);
            return user.Role;
        }
    }
}