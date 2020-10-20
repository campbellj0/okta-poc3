using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;

namespace OktaSampleApi.Swagger
{
    public class AddHeaderOperationFilter : IOperationFilter
    {
        // NonBodyParameter equivalent in net core 3.1 and swashbuckle 5
        // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1373

        private readonly string parameterName;
        private readonly string description;

        public AddHeaderOperationFilter(string parameterName, string description)
        {
            this.parameterName = parameterName;
            this.description = description;
        }

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            //operation.Parameters.Add(new NonBodyParameter
            //{
            //    Name = parameterName,
            //    In = "header",
            //    Description = description,
            //    Required = false,
            //    Type = "string"
            //});

            operation.Parameters?.Add(new OpenApiParameter
            {
                Name = parameterName,
                In = ParameterLocation.Header,
                Description = description,
                Required = false,
                Schema = new OpenApiSchema { Type = "string" }
            });
        }
    }

    public class AuthorizationInputOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
            {
                operation.Parameters = new List<OpenApiParameter>();
            }

            //operation.Parameters.Add(new NonBodyParameter
            //{
            //    Name = "ApiKey",
            //    In = "header",
            //    Description = "access token",
            //    Required = false,
            //    Type = "apikey"
            //});

            operation.Parameters?.Add(new OpenApiParameter
            {
                Name = "ApiKey",
                In = ParameterLocation.Header,
                Description = "access token",
                Required = false,
                Schema = new OpenApiSchema { Type = "apikey" }
            });
        }
    }
}
