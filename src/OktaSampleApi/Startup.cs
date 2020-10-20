using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Okta.AspNetCore;
using OktaSampleApi.Services;
using OktaSampleApi.Swagger;

namespace OktaSampleApi
{
    public class Startup
    {
        private readonly ILogger<Startup> _logger;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            _logger = loggerFactory.CreateLogger<Startup>();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddCors(options =>
            //{
            //    // The CORS policy is open for testing purposes. 
            //    // In a production application, you should restrict it to known origins.
            //    options.AddPolicy(
            //        "AllowAll",
            //        builder => builder.AllowAnyOrigin()
            //        .AllowAnyMethod()
            //        .AllowAnyHeader());
            //});

            //use this portion to require authentication for everything
            services.AddMvc(o =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                o.Filters.Add(new AuthorizeFilter(policy));
            });

            //read from azure  app config service
            var oktaDomain = Configuration["OktaSampleApi:Settings:Okta:OktaDomain"];
            _logger.LogDebug($"oktaDomain = {oktaDomain}");

            var authorizationServerId = Configuration["OktaSampleApi:Settings:Okta:AuthorizationServerId"];
            _logger.LogDebug($"authorizationServerId = {authorizationServerId}");

            var audience = Configuration["OktaSampleApi:Settings:Okta:Audience"];
            _logger.LogDebug($"audience = {audience}");


            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = OktaDefaults.ApiAuthenticationScheme;
                options.DefaultChallengeScheme = OktaDefaults.ApiAuthenticationScheme;
                options.DefaultSignInScheme = OktaDefaults.ApiAuthenticationScheme;
            })
            .AddOktaWebApi(new OktaWebApiOptions()
            {
                OktaDomain = oktaDomain,
                AuthorizationServerId = authorizationServerId,
                Audience = audience
            });

            services.AddAuthorization();

            services.AddControllers();

            services.AddHttpContextAccessor();
            services.AddTransient<ICurrentUserService, CurrentUserService>();
            
            //add swagger
            services.AddSwaggerServices(Configuration);

            // add app insights
            services.AddAppInsights(
                Configuration["APPINSIGHTS_INSTRUMENTATIONKEY"],
                Configuration["APPINSIGHTS_CLOUDROLENAME"]);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            //use swagger
            app.UseSwaggerServices();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
