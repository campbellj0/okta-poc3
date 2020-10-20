using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace OktaSampleCommon.Logging
{
    public class CustomTelemetryInitializer : ITelemetryInitializer
    {
        private readonly string _cloudRoleName;
        public CustomTelemetryInitializer(string cloudRoleName)
        {
            _cloudRoleName = cloudRoleName;
        }

        public void Initialize(ITelemetry telemetry)
        {
            telemetry.Context.Cloud.RoleName = _cloudRoleName;
        }
    }
}
