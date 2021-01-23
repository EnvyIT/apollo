using System;
using System.Security.Authentication;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace Apollo.Util.Test
{
    public class JwtUtilTest
    {
        private const string ValidJwt = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJhcG9sbG8gdGVzdCIsImlhdCI6MTYwNjE2NDQ0MiwiZXhwIjoxNjM3NzAwNDQyLCJhdWQiOiJhcG9sbG8gdGVzdCIsInN1YiI6ImFwb2xsbyB0ZXN0IiwidXVpZCI6IjczMmQxNDlhLWU2Y2YtNDg2Yy1iODY4LWVlYzAzYmIyZGJlNiJ9.7__Qet6_psCN4beoRcvnZ-RoEokTqOVwlK0POySVhqM";
        private const string ExpiredJwt = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJhcG9sbG8gdGVzdCIsImlhdCI6MTYwNjE2NDQ0MiwiZXhwIjoxNTc0NTQyMDQyLCJhdWQiOiJhcG9sbG8gdGVzdCIsInN1YiI6ImFwb2xsbyB0ZXN0IiwidXVpZCI6IjczMmQxNDlhLWU2Y2YtNDg2Yy1iODY4LWVlYzAzYmIyZGJlNiJ9.-Sx1rbrlDsmkZ4lkZsR7EKDBdoQqsY-ZOTCz-yuhPwY";
        private const string InvalidIssuerJwt = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJhcG9sbG8iLCJpYXQiOjE2MDYxNjQ0NDIsImV4cCI6MTYzNzcwMDQ0MiwiYXVkIjoiYXBvbGxvIHRlc3QiLCJzdWIiOiJhcG9sbG8gdGVzdCIsInV1aWQiOiI3MzJkMTQ5YS1lNmNmLTQ4NmMtYjg2OC1lZWMwM2JiMmRiZTYifQ.cCFEh-zGMsZyHL21A-oI6k8hy1AUs4MYOFW9eex38sw";
        private const string InvalidAudienceJwt = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJhcG9sbG8gdGVzdCIsImlhdCI6MTYwNjE2NDQ0MiwiZXhwIjoxNjM3NzAwNDQyLCJhdWQiOiJhcG9sbG8iLCJzdWIiOiJhcG9sbG8gdGVzdCIsInV1aWQiOiI3MzJkMTQ5YS1lNmNmLTQ4NmMtYjg2OC1lZWMwM2JiMmRiZTYifQ.1MO6mK-oDo2gyHgSDY8FhlAHx1zkIqTSH0-_RVfOV1E";
        private const string InvalidSecretKeyJwt = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJhcG9sbG8gdGVzdCIsImlhdCI6MTYwNjE2NDQ0MiwiZXhwIjoxNjM3NzAwNDQyLCJhdWQiOiJhcG9sbG8gdGVzdCIsInN1YiI6ImFwb2xsbyB0ZXN0IiwidXVpZCI6IjczMmQxNDlhLWU2Y2YtNDg2Yy1iODY4LWVlYzAzYmIyZGJlNiJ9.6CERIrD4XykJoN3FA6HhdNWPEVHsmSvkacbp56bkPwQ";
        private const string UuidMissingJwt = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJhcG9sbG8gdGVzdCIsImlhdCI6MTYwNjE2NDQ0MiwiZXhwIjoxNjM3NzAwNDQyLCJhdWQiOiJhcG9sbG8gdGVzdCIsInN1YiI6ImFwb2xsbyB0ZXN0In0.vMu9R2c0dSpI7oWKpO8awQY7jkzmBfp6UxYUvcmwWTs";

        private const string Uuid = "732d149a-e6cf-486c-b868-eec03bb2dbe6";

        private const string UserName = "test1";
        private const string Password = "password123";

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            ConfigurationHelper.ConfigurationRoot = new ConfigurationBuilder()
                .AddJsonFile("config.test.json")
                .Build();
        }


        [Test]
        public void Test_IsJwtValid_Valid_ReturnTrue()
        {
            var result = JwtHelper.IsJwtValid(ValidJwt);
            result.Should().BeTrue();
        }

        [Test]
        public void Test_IsJwtValid_UuidMissing_ReturnTrue()
        {
            var result = JwtHelper.IsJwtValid(UuidMissingJwt);
            result.Should().BeTrue();
        }

        [Test]
        public void Test_IsJwtValid_ParameterNull_ShouldThrow_Exception()
        {
            Action callback = () => JwtHelper.IsJwtValid(null);
            callback.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Test_IsJwtValid_InvalidExpired_ReturnFalse()
        {
            var result = JwtHelper.IsJwtValid(ExpiredJwt);
            result.Should().BeFalse();
        }

        [Test]
        public void Test_IsJwtValid_InvalidIssuer_ReturnFalse()
        {
            var result = JwtHelper.IsJwtValid(InvalidIssuerJwt);
            result.Should().BeFalse();
        }

        [Test]
        public void Test_IsJwtValid_InvalidAudience_ReturnFalse()
        {
            var result = JwtHelper.IsJwtValid(InvalidAudienceJwt);
            result.Should().BeFalse();
        }

        [Test]
        public void Test_IsJwtValid_InvalidSecretKey_ReturnFalse()
        {
            var result = JwtHelper.IsJwtValid(InvalidSecretKeyJwt);
            result.Should().BeFalse();
        }

        [Test]
        public void Test_GetUuidFromJwt_Valid_ReturnUuid()
        {
            var result = JwtHelper.GetUuidFromJwt(ValidJwt);
            result.Should().Be(Uuid);
        }

        [Test]
        public void Test_GetUuidFromJwt_ParameterNull_ShouldThrow_Exception()
        {
            Action callback = () => JwtHelper.GetUuidFromJwt(null);
            callback.Should().Throw<ArgumentNullException>();
        }

        [Test]
        public void Test_GetUuidFromJwt_MissingProperty_ShouldThrow_Exception()
        {
            Action callback = () => JwtHelper.GetUuidFromJwt(UuidMissingJwt);
            callback.Should().Throw<ArgumentException>();
        }

        [Test]
        public void Test_GetUuidFromJwt_InvalidJwt_ShouldThrow_Exception()
        {
            Action callback = () => JwtHelper.GetUuidFromJwt(ExpiredJwt);
            callback.Should().Throw<ArgumentException>();
        }

        [Test]
        public async Task Test_GetToken_ValidCredentials_ShouldReturn_Jwt()
        {
            var jwt = await JwtHelper.GetTokenFromServerAsync(UserName, Password);
            jwt.Should().NotBeNull();
            jwt.Length.Should().BeGreaterThan(10);
        }

        [Test]
        public async Task Test_GetToken_InvalidCredentials_ShouldThrow_Exception()
        {
            Func<Task> callback = async () => await JwtHelper.GetTokenFromServerAsync(UserName, "invalid");
            await callback.Should().ThrowAsync<InvalidCredentialException>();
        }

        [Test]
        public async Task Test_ValidateToken_ValidToken_ShouldReturn_Uuid()
        {
            var token = await JwtHelper.GetTokenFromServerAsync(UserName, Password);
            var result = await JwtHelper.IsTokenValidFromServerAsync(token);

            result.Should().NotBeNull();
            result.IsValid.Should().BeTrue();
            result.Content.Uuid.Length.Should().Be(36);
        }

        [Test]
        public async Task Test_GetToken_InvalidToken_ShouldReturn_Invalid()
        {
            var result = await JwtHelper.IsTokenValidFromServerAsync(ExpiredJwt);

            result.Should().NotBeNull();
            result.IsValid.Should().BeFalse();
            result.Content.Should().BeNull();
        }

        [Test]
        public async Task Test_GetToken_InvalidUrl_ShouldThrow_Exception()
        {
            ConfigurationHelper.ConfigurationRoot = new ConfigurationBuilder()
                .AddJsonFile("invalid.config.test.json")
                .AddJsonFile("config.test.json")
                .Build();
            Func<Task> callback = async () => await JwtHelper.GetTokenFromServerAsync(UserName, "invalid");
            await callback.Should().ThrowAsync<InvalidCredentialException>();
        }
    }
}