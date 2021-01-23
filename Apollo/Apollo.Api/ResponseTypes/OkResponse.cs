using System.Net;

namespace Apollo.Api.ResponseTypes
{
    public class OkResponse: ApiResponse
    {
        public OkResponse() : base(HttpStatusCode.OK)
        {
        }

        public OkResponse(string message) : base(HttpStatusCode.OK, message)
        {
        }
    }
}
