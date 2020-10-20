using Azure.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography.X509Certificates;

[assembly: HostingStartup(typeof(OktaSampleCommon.ServiceKeyInjection))]
namespace OktaSampleCommon
{
    public class ServiceKeyInjection : IHostingStartup
    {
        public IConfiguration Configuration { get; private set; }
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration((hostingContext, config) =>
            {
                var settings = config.Build();

                config.AddAzureAppConfiguration(options =>
                {
                    options.Connect(settings["ConnectionStrings:AppConfig"])
                        // Load configuration values with no label
                        .Select(KeyFilter.Any, LabelFilter.Null)
                        // Override with any configuration values specific to current hosting env
                        .Select(KeyFilter.Any, hostingContext.HostingEnvironment.EnvironmentName)
                        .Select(KeyFilter.Any, "Dev-Certificate")
                        .ConfigureKeyVault(kv =>
                        {
                            kv.SetCredential(new DefaultAzureCredential());
                        });
                });

                var dict = new Dictionary<string, string>
                {
                    {"DevAccount_FromLibrary", "DEV_1111111-1111"},
                    {"ProdAccount_FromLibrary", "PROD_2222222-2222"}
                };

                config.AddInMemoryCollection(dict);
            });

            builder.ConfigureKestrel((hostingContext, serverOptions) =>
            {
                Configuration = hostingContext.Configuration;
                //var config = new CertificateConfiguration
                //{
                //    UseLocalCertStore = Convert.ToBoolean(Configuration["CertificateConfiguration:UseLocalCertStore"]),
                //    CertificateThumbprint = Configuration["CertificateConfiguration:CertificateThumbprint"],
                //    DevelopmentCertificatePfx = Configuration["CertificateConfiguration:DevelopmentCertificatePfx"],
                //    DevelopmentCertificatePassword = Configuration["CertificateConfiguration:DevelopmentCertificatePassword"],
                //    KeyVaultEndpoint = Configuration["CertificateConfiguration:KeyVaultEndpoint"],
                //    CertificateNameKeyVault = Configuration["CertificateConfiguration:CertificateNameKeyVault"]
                //};

                var certificateKey = Environment.GetEnvironmentVariable("CERTIFICATE_KEY");

                serverOptions.ConfigureHttpsDefaults(options =>
                {
                    // read cert secret from app configuration with reference to keyvault
                    var certificateSecret = Configuration[certificateKey];
                    var secretValue = Convert.FromBase64String(certificateSecret);
                    var certificate = new X509Certificate2(secretValue);

                    options.ServerCertificate = certificate;
                });
                ////var port = int.Parse(Environment.GetEnvironmentVariable("ASPNETCORE_HTTPS_PORT"));
                //var port = int.Parse(Environment.GetEnvironmentVariable("ASPNETCORE_HTTPS_PORT"));
                //serverOptions.Listen(IPAddress.IPv6Any, port, listenOptions =>
                //{
                //    // read cert secret from app configuration with reference to keyvault
                //    var certificateSecret = Configuration[certificateKey];
                //    var secretValue = Convert.FromBase64String(certificateSecret);
                //    var certificate = new X509Certificate2(secretValue);
                //    listenOptions.UseHttps(certificate);

                //    //var certs = CertificateService.GetCertificates(config).GetAwaiter().GetResult();
                //    //listenOptions.UseHttps(certs.ActiveCertificate);
                //});
            });
        }
    }
}
