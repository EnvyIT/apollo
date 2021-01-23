using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Apollo.Util;
using Apollo.Util.Logger;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NUnit.Framework;
using static System.String;

namespace Apollo.Api.Test
{
    public class BaseApiTest : ApiWebApplicationFactory
    {
        public const string TerminalUser = "terminal";
        public const string TerminalPassword = "password123";

        public const string AdminUser = "admin";
        public const string AdminPassword = "admin123";

        private const string Api = "api";
        private const string ApplicationJson = "application/json";
        private const string AuthenticationKey = "AUTHENTICATION_ENDPOINT";
        private const string JwtEndpointKey = "JWT_ENDPOINT";
        private const string ApiVersionKey = "API_VERSION";

        private const string Authorization = "Authorization";
        private const string Bearer = "Bearer";

        private const string ParameterPage = "page";
        private const string ParameterPageSize = "pageSize";

        public HttpClient Client { get; set; }
        public IDataSeeder DataSeeder { get; private set; }

        public string AuthenticationEndpoint { get; private set; }
        public string ApiVersion { get; private set; }
        public AuthenticationResponse AuthenticationResponse { get; private set; }

        [OneTimeSetUp]
        public void OneTimeSetup()
        { 
            ConfigurationHelper.ConfigurationRoot = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            DataSeeder = new DataSeeder();
            var configurations = ConfigurationHelper.GetValues(JwtEndpointKey, AuthenticationKey, ApiVersionKey);
            AuthenticationEndpoint = $"{configurations[0]}{configurations[1]}";
            ApiVersion = configurations[2];
            Client = CreateTestClient();

            Client.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue(ApplicationJson));
        }

        [SetUp]
        public async Task Setup()
        {
            await DataSeeder.SeedDatabaseAsync();
        }

        [TearDown]
        public async Task TearDown()
        {
            await DataSeeder.ResetDatabaseAsync();
            Client.DefaultRequestHeaders.Remove(Authorization);
        }

        protected async Task Authenticate(string username = AdminUser, string password = AdminPassword)
        {
            await PerformAuthentication(username, password);
            if (Client.DefaultRequestHeaders.TryGetValues(Authorization, out var values))
            {
                Client.DefaultRequestHeaders.Remove(Authorization);
            }

            Client.DefaultRequestHeaders.Add(Authorization, $"{Bearer} {AuthenticationResponse.AccessToken}");
        }

        private async Task PerformAuthentication(string username, string password)
        {
            var httpClient = new HttpClient();
            var formData = new Dictionary<string, string>
            {
                {"client_id", "apollo"},
                {"username", username},
                {"password", password},
                {"grant_type", "password"}
            };
            var response = await httpClient.PostAsync(AuthenticationEndpoint, new FormUrlEncodedContent(formData));
            if (response.StatusCode == HttpStatusCode.OK)
            {
                AuthenticationResponse =
                    JsonConvert.DeserializeObject<AuthenticationResponse>(await response.Content.ReadAsStringAsync());
            }
        }

        protected Task<HttpResponseMessage> GetQuery(string endpoint, string method = null,
            IEnumerable<KeyValuePair<string, string>> urlParams = null)
        {
            var uri = Uri.EscapeUriString(
                $"{Api}/{ApiVersion}/{endpoint}{GetMethodUrlPart(method)}{urlParams.JoinKeyValuePairs()}");
            return Client.GetAsync(uri);
        }

        protected Task<HttpResponseMessage> Get(string endpoint, string method = null,
            IEnumerable<KeyValuePair<string, string>> endpointParams = null)
        {
            var uri = Uri.EscapeUriString(
                $"{Api}/{ApiVersion}/{endpoint}{GetMethodUrlPart(method)}{endpointParams.JoinKeyValuePairs(Empty, "/", "/")}");
            return Client.GetAsync(uri);
        }

        protected Task<HttpResponseMessage> Get(string endpoint, IEnumerable<KeyValuePair<string, string>> endpointParams = null)
        {
            var uri = Uri.EscapeUriString(
                $"{Api}/{ApiVersion}/{endpoint}/{endpointParams.JoinKeyValuePairs(Empty, "/", "/")}");
            return Client.GetAsync(uri);
        }

        protected Task<HttpResponseMessage> Post<T>(string endpoint, string method, T payload = default,
            IEnumerable<KeyValuePair<string, string>> endpointParams = null)
        {
            var uri = Uri.EscapeUriString(
                $"{Api}/{ApiVersion}/{endpoint}{GetMethodUrlPart(method)}{endpointParams.JoinKeyValuePairs()}");
            var jsonContent = JsonConvert.SerializeObject(payload);
            if (Equals(payload, default(T)))
            {
                jsonContent = Empty;
            }

            return Client.PostAsync(uri, new StringContent(jsonContent, Encoding.UTF8, ApplicationJson));
        }

        protected Task<HttpResponseMessage> Post<T>(string endpoint, T payload = default,
            IEnumerable<KeyValuePair<string, string>> endpointParams = null)
        {
            var uri = Uri.EscapeUriString(
                $"{Api}/{ApiVersion}/{endpoint}/{endpointParams.JoinKeyValuePairs()}");
            var jsonContent = JsonConvert.SerializeObject(payload);
            if (Equals(payload, default(T)))
            {
                jsonContent = Empty;
            }

            return Client.PostAsync(uri, new StringContent(jsonContent, Encoding.UTF8, ApplicationJson));
        }

        protected  Task<HttpResponseMessage> Put<T>(string endpoint, string method)
        {
            return Put<T>(endpoint, method, default(T));
        }

        protected Task<HttpResponseMessage> Put<T>(string endpoint, string method, T payload,
            IEnumerable<KeyValuePair<string, string>> endpointParams = null)
        {
            var uri = Uri.EscapeUriString(
                $"{Api}/{ApiVersion}/{endpoint}{GetMethodUrlPart(method)}{endpointParams.JoinKeyValuePairs()}");
            var jsonContent = JsonConvert.SerializeObject(payload);
            return Client.PutAsync(uri, new StringContent(jsonContent, Encoding.UTF8, ApplicationJson));
        }

        protected Task<HttpResponseMessage> Put<T>(string endpoint, T payload,
            IEnumerable<KeyValuePair<string, string>> endpointParams = null)
        {
            var uri = Uri.EscapeUriString(
                $"{Api}/{ApiVersion}/{endpoint}/{endpointParams.JoinKeyValuePairs()}");
            var jsonContent = JsonConvert.SerializeObject(payload);
            return Client.PutAsync(uri, new StringContent(jsonContent, Encoding.UTF8, ApplicationJson));
        }

        protected Task<HttpResponseMessage> Delete(string endpoint, string method = null,
            IEnumerable<KeyValuePair<string, string>> endpointParams = null)
        {
            var uri = Uri.EscapeUriString(
                $"{Api}/{ApiVersion}/{endpoint}{GetMethodUrlPart(method)}{endpointParams.JoinKeyValuePairs()}");
            return Client.DeleteAsync(uri);
        }

        private static string GetMethodUrlPart(string method)
        {
            return IsNullOrEmpty(method) ? Empty : $"/{method}/";
        }

        protected IEnumerable<KeyValuePair<string, string>> GetPageParameter(int page, int pageSize)
        {
            return new[]
            {
                new KeyValuePair<string, string>(ParameterPage, page.ToString()),
                new KeyValuePair<string, string>(ParameterPageSize, pageSize.ToString())
            };
        }

        protected async Task CheckResponseAndLogStatusCodeAsync<T>(IApolloLogger<T> logger,
            HttpResponseMessage response,
            HttpStatusCode expected = HttpStatusCode.OK)
        {
            if (response.StatusCode != expected)
            {
                logger.Error($"{response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
            }

            response.StatusCode.Should().Be(expected);
            logger.Info($"StatusCode: {response.StatusCode}");
        }

        protected async Task<long> GetCreatedIdAsync(HttpResponseMessage response)
        {
            return JsonConvert.DeserializeObject<CreatedResult>(await response.Content.ReadAsStringAsync()).Id;
        }

        protected static void ValidateDateTimes(DateTime createdScheduleStartTime, DateTime other)
        {
            createdScheduleStartTime.Should().HaveDay(other.Day);
            createdScheduleStartTime.Should().HaveMonth(other.Month);
            createdScheduleStartTime.Should().HaveYear(other.Year);
            createdScheduleStartTime.Should().HaveHour(other.Hour);
            createdScheduleStartTime.Should().HaveMinute(other.Minute);
            createdScheduleStartTime.Should().HaveSecond(other.Second);
        }

        private class CreatedResult
        {
            public long Id { get; set; }
        }
    }

    public class AuthenticationResponse
    {
        [JsonProperty("access_token")] public string AccessToken { get; set; }

        [JsonProperty("expires_in")] public int ExpiresIn { get; set; }

        [JsonProperty("refresh_expires_in")] public int RefreshExpiresIn { get; set; }

        [JsonProperty("refresh_token")] public string RefreshToken { get; set; }

        [JsonProperty("token_type")] public string TokenType { get; set; }

        [JsonProperty("not-before-policy")] public int NotBeforePolicy { get; set; }

        [JsonProperty("session_state")] public string SessionState { get; set; }

        [JsonProperty("scope")] public string Scope { get; set; }
    }
}