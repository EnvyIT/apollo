using System.Net;
using Newtonsoft.Json;

namespace Apollo.Api.ResponseTypes
{
    public class ApiResponse
    {
        [JsonIgnore]
        public HttpStatusCode StatusCode { get; }

        public string StatusDescription { get; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; }

        public ApiResponse(HttpStatusCode statusCode)
        {
            StatusCode = statusCode;
            StatusDescription = statusCode.ToString();
        }

        public ApiResponse(HttpStatusCode statusCode, string message) : this(statusCode)
        {
            Message = message;
        }
    }
}