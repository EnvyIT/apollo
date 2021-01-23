using System.Net;

namespace Apollo.Api.ResponseTypes
{
    public class BadRequestResponse : ApiResponse
    {
        public BadRequestResponse() : base(HttpStatusCode.BadRequest)
        {
        }

        public BadRequestResponse(string message) : base(HttpStatusCode.BadRequest, message)
        {
        }
    }
}