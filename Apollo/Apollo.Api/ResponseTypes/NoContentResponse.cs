using System.Net;

namespace Apollo.Api.ResponseTypes
{
    public class NoContentResponse: ApiResponse
    {
        public NoContentResponse() : base(HttpStatusCode.NoContent)
        {
        }

        public NoContentResponse(string message) : base(HttpStatusCode.NoContent, message)
        {
        }
    }
}
