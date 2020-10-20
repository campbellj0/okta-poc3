namespace OktaSampleApi.Swagger
{
    public interface ISwaggerSettings
    {
        bool UseSwagger { get; set; }
        string DocumentName { get; set; }
        string Version { get; set; }
        string Title { get; set; }
        string Description { get; set; }
        string TermsOfService { get; set; }
        string ContactName { get; set; }
        string ContactEmail { get; set; }
        string ContactUrl { get; set; }
        string LicenseName { get; set; }
        string LicenseUrl { get; set; }
        string RouteTemplate { get; set; }
        string UiRoutePrefix { get; set; }
        string UiEndpointUrl { get; set; }
        string UiEndpointDescription { get; set; }
        bool ApiKeyProtected { get; set; }
        string ApiKeyHeaderName { get; set; }
    }
}
