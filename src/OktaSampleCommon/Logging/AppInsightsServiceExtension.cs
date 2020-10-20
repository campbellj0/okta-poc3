using Microsoft.ApplicationInsights.Extensibility;
using OktaSampleCommon.Logging;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AppInsightsServiceExtension
    {
        public static IServiceCollection AddAppInsights(this IServiceCollection services, string instrumentationKey, string cloudRoleName = null)
        {
            if (!string.IsNullOrWhiteSpace(cloudRoleName))
            {
                services.AddSingleton<ITelemetryInitializer>(new CustomTelemetryInitializer(cloudRoleName));
            }
            services.AddApplicationInsightsTelemetry(instrumentationKey);
            return services;
        }
    }
}
