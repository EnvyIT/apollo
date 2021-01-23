using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using Apollo.Api.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Apollo.Api.Filters
{
    public class AuthorizeRoleOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Method attributes
            var attributes = GetAttributes(context.MethodInfo.GetCustomAttributes(true)).ToList();

            if (!attributes.Any())
            {
                // Class attributes
                attributes = GetAttributes(context.MethodInfo.DeclaringType?.GetCustomAttributes(typeof(AuthorizeRoleAttribute))).ToList();
            }

            if (!attributes.Any())
            {
                return;
            }

            AddDescription(operation, attributes);
            AddResponseType(HttpStatusCode.Unauthorized, operation.Responses.TryAdd);
            AddResponseType(HttpStatusCode.Forbidden, operation.Responses.TryAdd);

            operation.Security = new List<OpenApiSecurityRequirement>
            {
                new OpenApiSecurityRequirement
                {
                    [GetSchema()] = new string[] { }
                }
            };
        }

        private IEnumerable<AuthorizeRoleAttribute> GetAttributes(IEnumerable<object> attributes)
        {
            return attributes
                .OfType<AuthorizeRoleAttribute>()
                .Distinct();
        }

        private void AddResponseType(HttpStatusCode statusCode, Func<string, OpenApiResponse, bool> addCallback)
        {
            addCallback(((int) statusCode).ToString(), new OpenApiResponse {Description = statusCode.ToString()});
        }

        private void AddDescription(OpenApiOperation operation, IEnumerable<AuthorizeRoleAttribute> attributes)
        {
            var roles = attributes
                .SelectMany(attribute => attribute.Roles)
                .Distinct()
                .ToList();
            if (roles.Contains(ApolloRoles.All))
            {
                roles = Enum.GetValues<ApolloRoles>()
                    .Where(role => role != ApolloRoles.All)
                    .ToList();
            }

            if (operation.Description != null && operation.Description.Length > 0)
            {
                operation.Description = $"{operation.Description}{Environment.NewLine}{Environment.NewLine}";
            }
            operation.Description = $"{operation.Description}Allowed roles:";
            foreach (var role in roles)
            {
                operation.Description = $"{operation.Description}{Environment.NewLine}- {role}";
            }
        }

        public static OpenApiSecurityScheme GetSchema()
        {
            return new OpenApiSecurityScheme
            {
                Name = "JWT Authentication",
                Description = "Enter JWT Bearer token **_only_**",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer", // must be lower case
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            };
        }
    }
}