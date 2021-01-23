using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Apollo.Util.Types;
using Microsoft.IdentityModel.Tokens;

namespace Apollo.Util
{
    public static class JwtHelper
    {
        private const string ClaimUuid = "uuid";

        private const string UrlUserInfo = "/protocol/openid-connect/userinfo";
        private const string UrlToken = "/protocol/openid-connect/token";

        private const string ParameterToken = "access_token";
        private const string ParameterClientId = "client_id";
        private const string ParameterUserName = "username";
        private const string ParameterPassword = "password";
        private const string ParameterGrantTypePassword = "grant_type=password";

        public static bool IsJwtValid(string jwt)
        {
            if (jwt == null)
            {
                throw new ArgumentNullException(nameof(jwt));
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters();

            try
            {
                tokenHandler.ValidateToken(jwt, validationParameters, out _);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string GetUuidFromJwt(string jwt)
        {
            if (!IsJwtValid(jwt))
            {
                throw new ArgumentException("Invalid JWT given!", nameof(jwt));
            }

            var token = new JwtSecurityToken(jwt);
            var uuid = GetUuidFromClaims(token.Claims);

            return uuid ?? throw new ArgumentException($"Claim {ClaimUuid} missing!");
        }

        public static string GetUuidFromClaims(IEnumerable<Claim> claims)
        {
            return claims.FirstOrDefault(claim => claim.Type == ClaimUuid)?.Value;
        }

        public static async Task<RequestResult<TokenClaims>> IsTokenValidFromServerAsync(string token)
        {
            var values = ConfigurationHelper.GetValues("JWT_ENDPOINT");
            var contentList = new List<string>
            {
                $"{ParameterToken}={token}"
            };

            var result = await ExecutePostCallPostAsync<UserInfoResult>($"{values[0]}{UrlUserInfo}", contentList);
            return result.IsValid
                ? new RequestResult<TokenClaims>(true, new TokenClaims(result.Content.Uuid))
                : new RequestResult<TokenClaims>(false, default);
        }

        public static async Task<string> GetTokenFromServerAsync(string userName, string password)
        {
            var values = ConfigurationHelper.GetValues("JWT_ENDPOINT", "JWT_CLIENT");
            var contentList = new List<string>
            {
                $"{ParameterClientId}={values[1]}", $"{ParameterUserName}={userName}",
                $"{ParameterPassword}={password}", ParameterGrantTypePassword
            };

            var result = await ExecutePostCallPostAsync<TokenResult>($"{values[0]}{UrlToken}", contentList);
            if (result.IsValid)
            {
                return result.Content.Access_Token;
            }

            throw new InvalidCredentialException("Invalid username or password given!");
        }

        private static TokenValidationParameters GetValidationParameters()
        {
            var values = ConfigurationHelper.GetValues("JWT_ISSUER", "JWT_AUDIENCE", "JWT_SECURITY_KEY");
            return new TokenValidationParameters
            {
                ValidateLifetime = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidIssuer = values[0],
                ValidAudience = values[1],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(values[2])
                )
            };
        }

        private static async Task<RequestResult<T>> ExecutePostCallPostAsync<T>(string url,
            IEnumerable<string> contentList)
        {
            using var httpClient = new HttpClient();
            using var request = new HttpRequestMessage(new HttpMethod("POST"), url)
            {
                Content = new StringContent(string.Join("&", contentList))
            };

            request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");

            var result = new RequestResult<T>(false, default);
            try
            {
                var response = await httpClient.SendAsync(request);
                result.IsValid = response.StatusCode == HttpStatusCode.OK;
                if (result.IsValid)
                {
                    result.Content = JsonMapper.Map<T>(await response.Content.ReadAsStringAsync());
                }
            }
            catch (Exception)
            {
                result.IsValid = false;
            }

            return result;
        }
    }
}