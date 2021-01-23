using System.Net.Http;
using Apollo.Core.Implementation;
using Apollo.Core.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Apollo.Api.Test
{
    public class ApiWebApplicationFactory : WebApplicationFactory<Startup>
    {

        public HttpClient CreateTestClient()
        {
            return WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddScoped<IServiceFactory>(container => new ServiceFactory("APOLLO_TEST"));
                });

                builder.ConfigureAppConfiguration((context, build) =>
                {
                    build.Sources.Clear();
                    var integrationConfig = new ConfigurationBuilder()
                        .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json")
                        .Build();

                    build.AddConfiguration(integrationConfig);
                });

            }).CreateClient();
        }
    }
}
