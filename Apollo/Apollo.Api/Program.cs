using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Apollo.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return new HostBuilder()
                .ConfigureLogging(builder => { builder.ClearProviders(); })
                .ConfigureAppConfiguration((context, builder) =>
                {
                    var configuration = builder
                        .AddEnvironmentVariables()
                        .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", false, true)
                        .Build();
                    builder.AddConfiguration(configuration);
                })
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }
}