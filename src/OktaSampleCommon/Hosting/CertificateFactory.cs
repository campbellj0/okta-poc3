using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace OktaSampleCommon
{
    public static class CertificateFactory
    {
        private static async Task<(X509Certificate2 ActiveCertificate, X509Certificate2 SecondaryCertificate)>
        GetCertificates(IWebHostEnvironment environment, IConfiguration configuration)
        {
            var certificateConfiguration = new CertificateConfiguration
            {
                // Use an Azure key vault
                CertificateNameKeyVault = configuration["CertificateNameKeyVault"],
                KeyVaultEndpoint = configuration["AzureKeyVaultEndpoint"],

                // development certificate
                DevelopmentCertificatePfx = Path.Combine(environment.ContentRootPath, "sts_dev_cert.pfx"),
                DevelopmentCertificatePassword = "1234" //configuration["DevelopmentCertificatePassword"] 
            };

            (X509Certificate2 ActiveCertificate, X509Certificate2 SecondaryCertificate)
                certs = await CertificateService.GetCertificates(
                certificateConfiguration).ConfigureAwait(false);

            return certs;
        }
    }
}
