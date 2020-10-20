using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using System;

namespace OktaSampleApi.Swagger
{
    public static class SwaggerExtension
    {
        public static IServiceCollection AddSwaggerServices(this IServiceCollection services, IConfiguration configuration, ILoggerFactory loggerFactory = null)
        {
            //read swagger settings
            var swaggerSettings = new SwaggerSettings();
            var section = configuration.GetSection("SwaggerSettings");
            services.Configure<ISwaggerSettings>(section);
            section.Bind(swaggerSettings);
            services.AddSingleton<ISwaggerSettings>(swaggerSettings);

            //add swagger
            services.AddSwaggerGenerator(swaggerSettings);
            return services;
        }

        private static IServiceCollection AddSwaggerGenerator(this IServiceCollection services, ISwaggerSettings settings)
        {
            //if swagger is enabled, register the swagger generator, defining one or more Swagger documents
            if (settings.UseSwagger)
            {
                services.AddSwaggerGen(options =>
                {
                    options.SwaggerDoc(settings.Version, new OpenApiInfo
                    {
                        Version = settings.Version,
                        Title = settings.Title,
                        Description = settings.Description,
                        TermsOfService = new Uri(settings.TermsOfService),
                        Contact = new OpenApiContact
                        {
                            Name = settings.ContactName,
                            Email = settings.ContactEmail,
                            //Url = settings.ContactUrl
                        },
                        License = new OpenApiLicense
                        {
                            Name = settings.LicenseName,
                            //Url = settings.LicenseUrl
                        }
                    });
                    if (settings.ApiKeyProtected)
                    {
                        options.OperationFilter<AddHeaderOperationFilter>(settings.ApiKeyHeaderName, "Enter Key");
                    }
                    //either one of these will work
                    //c.OperationFilter<AuthorizationInputOperationFilter>();
                    
                });
            }
            return services;
        }

        public static IApplicationBuilder UseSwaggerServices(this IApplicationBuilder builder)
        {
            //get settings from container
            var settings = builder.ApplicationServices.GetRequiredService<ISwaggerSettings>();
            if (!settings.UseSwagger) return builder;

            //enable middleware to serve generated swagger as a JSON endpoint.
            builder.UseSwagger(c =>
            {
                //Change the path of the end point , should also update UI middle ware for this change                
                c.RouteTemplate = settings.RouteTemplate;
            });
            builder.UseSwaggerUI(c =>
            {
                //Include virtual directory if site is configured so
                if (!string.IsNullOrWhiteSpace(settings.UiRoutePrefix))
                {
                    c.RoutePrefix = settings.UiRoutePrefix;
                }
                c.SwaggerEndpoint(settings.UiEndpointUrl, settings.UiEndpointDescription);
            });

            ////enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            //builder.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("/swagger/v3/swagger.json", "VAPI V3");
            //});
            return builder;
        }
    }
}
