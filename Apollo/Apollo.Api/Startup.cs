using System;
using System.IO;
using System.Reflection;
using Apollo.Api.Authorization;
using Apollo.Api.Filters;
using Apollo.Core.Implementation;
using Apollo.Core.Interfaces;
using Apollo.Util;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace Apollo.Api
{
    public class Startup
    {
        private const string AUTHENTICATION_AUTHORITY_KEY = "JWT_ENDPOINT";
        private const string AUTHENTICATION_AUTHORITY_KEY_LOCAL = "JWT_ENDPOINT_LOCAL";
        private const string AUTHENTICATION_AUDIENCE_KEY = "JWT_CLIENT";


        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ConfigurationHelper.ConfigurationRoot = (IConfigurationRoot) Configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(config =>
                {
                    config.ReturnHttpNotAcceptable = true;
                    config.Filters.Add(typeof(ExceptionFilter));
                    config.SuppressAsyncSuffixInActionNames = false;
                })
                .AddNewtonsoftJson()
                .AddXmlDataContractSerializerFormatters();
            services.AddRouting(options => options.LowercaseUrls = true);
            services.AddScoped<IServiceFactory>(container => new ServiceFactory("Apollo_Database"));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Apollo API",
                    Description = "The legendary Apollo cinema API.",
                    TermsOfService = new Uri("https://example.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Rudolf Hermanseder and Michael Eder",
                        Email = "info@fh-hagenberg.at",
                        Url = new Uri("https://www.fh-ooe.at/en/hagenberg-campus/")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT",
                        Url = new Uri("https://opensource.org/licenses/MIT"),
                    }
                });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                var securityScheme = AuthorizeRoleOperationFilter.GetSchema();
                c.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
                c.OperationFilter<AuthorizeRoleOperationFilter>();
            });

            var config = ConfigurationHelper.GetValues(AUTHENTICATION_AUTHORITY_KEY,
                AUTHENTICATION_AUTHORITY_KEY_LOCAL, AUTHENTICATION_AUDIENCE_KEY
            );
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.Authority = config[0];
                    options.Audience = config[2];
                    options.TokenValidationParameters.ValidIssuers = new[]
                    {
                        config[0],
                        config[1]
                    };
                });
            services.AddSingleton<IAuthorizationPolicyProvider, RolePolicyProvider>();
            services.AddSingleton<IAuthorizationHandler, RolesAuthorizationHandler>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Apollo.Api v1"));
            app.UseCors(config => config.AllowAnyOrigin());
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}