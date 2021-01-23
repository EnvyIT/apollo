using System.Net;

namespace Apollo.Api.ResponseTypes
{
    public class NotFoundResponse: ApiResponse
    {
        public NotFoundResponse() : base(HttpStatusCode.NotFound)
        {
        }

        public NotFoundResponse(string message) : base(HttpStatusCode.NotFound, message)
        {
        }
    }
}
