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
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseKestrel(options =>
                        {
                            options.Listen(IPAddress.Any, 80);

                            var certificate = GetCertificate();
                            if (certificate != null)
                            {
                                options.Listen(IPAddress.Any, 443, listenOptions =>
                                {
                                    listenOptions.UseHttps(certificate);
                                });
                            }
                        }
                    );
                });
        }

        private static X509Certificate2? GetCertificate()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("certificates-settings.json", optional: true, reloadOnChange: true)
                .Build();

            var certificatesSettings = config.GetSection("certificatesSettings");
            string? certificatePath = certificatesSettings.GetValue<string>("certificatePath");
            string? passwordPath = certificatesSettings.GetValue<string>("passwordPath");

            if (certificatePath.IsNullOrEmpty() || passwordPath.IsNullOrEmpty()
                || !File.Exists(passwordPath) || !File.Exists(certificatePath))
            {
                return null;
            }

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
