using Apollo.Api.ResponseTypes;
using Apollo.Core.Dto;
using Microsoft.AspNetCore.Mvc;

namespace Apollo.Api.Controllers.Base
{
    [ApiController]
    [Route("api/" + ApiVersion + "/[controller]")]
    [Produces("application/json", "application/xml")]
    public class ApiControllerBase : ControllerBase
    {
        protected const string ApiVersion = "v1";

        protected IActionResult GetDeletionStatus(bool deleted, string errorMessage = null)
        {
            return deleted ? DeletedResponse() : BadRequestResponse(errorMessage);
        }

        protected IActionResult GetUpdatedStatus<T>(T result, string errorMessage = null)
            where T : BaseDto
        {
            return result == null ? BadRequestResponse(errorMessage) : UpdatedResponse();
        }

        protected IActionResult GetCreatedStatus<T>(T result, string resourceActionName, string errorMessage = null)
            where T : BaseDto
        {
            return result == null ? BadRequestResponse(errorMessage) : CreatedIdResponse(resourceActionName, result.Id);
        }

        protected ActionResult<T> GetUpdateStatus<T>(T updatedEntity)
        {
            if (updatedEntity == null)
            {
                return BadRequestResponse($"{typeof(T).Name} could not be updated");
            }

            return updatedEntity;
        }

        protected ObjectResult OkResponse(string message = null)
        {
            return Ok(new OkResponse(message));
        }

        protected ObjectResult NoContentResponse(string message = null)
        {
            return CreateCustomObjectResult(new NoContentResponse(message));
        }

        protected ObjectResult UpdatedResponse(string message = null)
        {
            return NoContentResponse(message);
        }

        protected ObjectResult DeletedResponse(string message = null)
        {
            return NoContentResponse(message);
        }

        protected ObjectResult CreatedIdResponse(string actionName = null, long id = 0L)
        {
            var response = new CreatedResponse(id);
            return actionName == null ? CreateCustomObjectResult(response) : CreatedAtAction(actionName, new { id }, response);
        }

        protected ObjectResult BadRequestResponse(string message = null)
        {
            return BadRequest(new BadRequestResponse(message));
        }

        protected ObjectResult NotFoundResponse(string message = null)
        {
            return BadRequest(new NotFoundResponse(message));
        }

        private static ObjectResult CreateCustomObjectResult(ApiResponse responseData)
        {
            return new ObjectResult(responseData) { StatusCode = (int)responseData.StatusCode };
        }
    }
}