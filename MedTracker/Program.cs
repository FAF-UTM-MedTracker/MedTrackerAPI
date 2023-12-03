using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Security.Cryptography.X509Certificates;


namespace MedTracker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = new WebHostBuilder();
            builder.UseKestrel(options =>
                {
                    options.AddServerHeader = false;
                    options.Listen(IPAddress.Any, 8080, listenOptions => { });

                    var certificate = GetCertificate();
                    if (certificate != null)
                        options.Listen(IPAddress.Any, 4432, listenOptions =>
                        {
                            listenOptions.UseHttps(certificate);
                        });
                });
            builder.UseStartup<Startup>();
            
            var host = builder.Build();
            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }

        private static X509Certificate2 GetCertificate()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("certificate.json", optional: true, reloadOnChange: true)
                .Build();

            var certificateSettings = config.GetSection("certificateSettings");
            string? certificatePath = certificateSettings.GetValue<string>("certificatePath");
            string? passwordPath = certificateSettings.GetValue<string>("passwordPath");

            if (certificatePath.IsNullOrEmpty() || passwordPath.IsNullOrEmpty())
                return null;

            var certificatePassword = GetFirstWord(ReadFileAsString(passwordPath));
            return new X509Certificate2(certificatePath, certificatePassword);
        }

        static string ReadFileAsString(string filePath)
        {
            var reader = new StreamReader(filePath);
            return reader.ReadToEnd();
        }

        static string GetFirstWord(string input)
        {
            // Split the input by spaces and take the first element
            string[] words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return words.Length > 0 ? words[0] : string.Empty;
        }
    }
}
