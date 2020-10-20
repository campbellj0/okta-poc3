using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Okta.AspNetCore;
using OktaSampleWeb.Services.Weather;
using System.Collections.Generic;

namespace OktaSampleWeb
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
            //read from azure  app config service
            var oktaDomain = Configuration["OktaSampleWeb:Settings:Okta:OktaDomain"];
            _logger.LogDebug($"oktaDomain = {oktaDomain}");

            var clientId = Configuration["OktaSampleWeb:Settings:Okta:ClientId"];
            _logger.LogDebug($"clientId = {clientId}");

            var clientSecret = Configuration["OktaSampleWeb:Settings:Okta:ClientSecret"];
            _logger.LogDebug($"clientSecret = {clientSecret}");

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
           .AddCookie()
           .AddOktaMvc(new OktaMvcOptions
           {
               // Replace these values with your Okta configuration
               OktaDomain = oktaDomain,
               ClientId = clientId,
               ClientSecret = clientSecret,
               Scope = new List<string> { "openid", "profile", "email" },
           });

            services.AddControllersWithViews();

            services.AddHttpContextAccessor();

            //add weather services
            services.AddWeatherService(Configuration);

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
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
