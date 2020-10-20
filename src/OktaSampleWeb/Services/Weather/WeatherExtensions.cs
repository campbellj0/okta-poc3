using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using System.Net.Http;

namespace OktaSampleWeb.Services.Weather
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWeatherService(this IServiceCollection services, IConfiguration configuration)
        {
            //get provider configurations - currently binding from appsettings.json
            var provider = GetWeatherProvider(services, configuration);

            // add rosebud service client
            AddWeatherApiClient(services, provider);

            return services;
        }

        private static IServiceCollection AddWeatherApiClient(IServiceCollection services, IWeatherProvider provider)
        {
            //var registry = services.AddPolicyRegistry();
            //var timeout = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10));
            //var longTimeout = Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(30));
            //registry.Add("regular", timeout);
            //registry.Add("long", longTimeout);

            services.AddHttpClient("weather-api", c =>
            {
                c.BaseAddress = new System.Uri(provider.BaseAddress);
                //c.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "prikey-sub-1-service-fabric-api-product");
                c.DefaultRequestHeaders.Add("Accept", "application/json");
            })

            // Build a totally custom policy using any criteria
            //.AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(TimeSpan.FromSeconds(10)))

            // Use a specific named policy from the registry. Simplest way, policy is cached for the
            // lifetime of the handler.
            //.AddPolicyHandlerFromRegistry("regular")

            // Run some code to select a policy based on the request
            //.AddPolicyHandler((request) =>
            //{
            //    return request.Method == HttpMethod.Get ? timeout : longTimeout;
            //})

            // Run some code to select a policy from the registry based on the request
            //.AddPolicyHandlerFromRegistry((reg, request) =>
            //{
            //    return request.Method == HttpMethod.Get ?
            //        reg.Get<IAsyncPolicy<HttpResponseMessage>>("regular") :
            //        reg.Get<IAsyncPolicy<HttpResponseMessage>>("long");
            //})

            // build a policy that will handle exceptions, 408s, and 500s from the remote server
            .AddTransientHttpErrorPolicy(p => p.RetryAsync())

            // retry requests using retry handler
            //.AddHttpMessageHandler(() => new RetryHandler())

            //add unauthorized retry handler
            //.AddHttpMessageHandler(() => new AuditHandler())

            //.AddHttpMessageHandler<AuthenticationHandler>()

            // bind address service to the named client "experian-address-api"
            .AddTypedClient<IWeatherService, WeatherService>()
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                return new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                    AllowAutoRedirect = false
                    //UseDefaultCredentials = true
                };
            });
            return services;
        }

        private static IWeatherProvider GetWeatherProvider(IServiceCollection services, IConfiguration configuration)
        {
            IWeatherProvider settings = new WeatherProvider();
            var section = configuration.GetSection("WeatherSettings");
            services.Configure<IWeatherProvider>(section);
            section.Bind(settings);
            services.AddSingleton(settings);
            return settings;

        }
    }
}
