using System.Net;
using System.Threading.Tasks;
using Apollo.Core.Dto;
using Apollo.Util.Logger;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Apollo.Api.Test
{
    public class UserControllerTest : BaseApiTest
    {
        private const string InvalidJwt =
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c";

        private const string UserEndpoint = "user";
        private const string GetRoleUrlPath = "role";

        private static readonly IApolloLogger<UserControllerTest> Logger =
            LoggerFactory.CreateLogger<UserControllerTest>();

        [Test]
        public async Task GetUserRole_ShouldReturnUserRoleForValidJwt()
        {
            await Authenticate(TerminalUser, TerminalPassword);

            var response = await Get(UserEndpoint, GetRoleUrlPath);
            await CheckResponseAndLogStatusCodeAsync(Logger, response);

            var role = JsonConvert.DeserializeObject<RoleDto>(await response.Content.ReadAsStringAsync());
            role.Label.Should().Be("Standard");
            role.MaxReservations.Should().Be(6);
        }

        [Test]
        public async Task GetUserRole_ShouldReturnInvalidJwt()
        {
            var response = await Get(UserEndpoint, GetRoleUrlPath);
            await CheckResponseAndLogStatusCodeAsync(Logger, response, HttpStatusCode.Unauthorized);
        }
    }
}