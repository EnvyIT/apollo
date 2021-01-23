using System.Net;

namespace Apollo.Api.ResponseTypes
{
    public class CreatedResponse : ApiResponse
    {
        public long Id { get; set; }

        public CreatedResponse() : base(HttpStatusCode.Created)
        {
        }

        public CreatedResponse(long id) : this()
        {
            Id = id;
        }
    }
}