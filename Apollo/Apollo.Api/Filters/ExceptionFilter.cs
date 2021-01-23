using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Apollo.Api.ResponseTypes;
using Apollo.Core.Exception;
using Apollo.Persistence.Exception;
using Apollo.Util;
using Apollo.Util.Logger;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


namespace Apollo.Api.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        private static readonly IApolloLogger<ExceptionFilter> Logger = LoggerFactory.CreateLogger<ExceptionFilter>();

        private class RuleDefinition
        {
            public HttpStatusCode StatusCode { get; set; }
            public string Message { get; set; }
            public Type[] ExceptionTypes { get; set; }
        }

        private readonly IList<RuleDefinition> _rules = new List<RuleDefinition>()
        {
            new RuleDefinition
            {
                StatusCode = HttpStatusCode.InternalServerError, Message = "Persistence error",
                ExceptionTypes = new[] {typeof(PersistenceException)}
            },
            new RuleDefinition
            {
                StatusCode = HttpStatusCode.BadRequest, Message = "Argument error",
                ExceptionTypes = new[]
                    {typeof(ArgumentException), typeof(ArgumentNullException), typeof(ArgumentOutOfRangeException)}
            },
            new RuleDefinition
            {
                StatusCode = HttpStatusCode.BadRequest, Message = "Invalid id given",
                ExceptionTypes = new[] {typeof(InvalidEntityIdException)}
            }
        };

        public void OnException(ExceptionContext context)
        {
            var definition = _rules
                .Where(rule => context.Exception.GetType().IsAnyType(rule.ExceptionTypes))
                .DefaultIfEmpty(new RuleDefinition {StatusCode = HttpStatusCode.InternalServerError, Message = "Unknown error"})
                .First();

            Logger.Error(context.Exception, "{StatusCode} - {Message}", definition.StatusCode, definition.Message);

            context.ExceptionHandled = true;
            context.HttpContext.Response.StatusCode = (int)definition.StatusCode;
            context.Result = new ObjectResult(new ApiResponse(definition.StatusCode, definition.Message));
        }
    }
}