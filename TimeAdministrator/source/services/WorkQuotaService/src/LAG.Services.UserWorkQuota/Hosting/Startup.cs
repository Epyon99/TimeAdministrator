using System;
using System.IO;
using EPY.Services.UserWorkQuota.Repositories;
using EPY.Services.UserWorkQuota.Repositories.Memory;
using EPY.Services.UserWorkQuota.Repositories.MongoDb;
using EPY.Services.UserWorkQuota.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EPY.Services.UserWorkQuota
{
#pragma warning disable SA1600 // Elements must be documented
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            if (env == null)
            {
                throw new ArgumentNullException(nameof(env));
            }

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            Configuration = builder.Build();
            Environment = env;
        }

        internal IConfigurationRoot Configuration { get; set; }

        internal IHostingEnvironment Environment { get; set; }

#pragma warning disable RECS0154 // Parameter is never used
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
#pragma warning restore RECS0154 // Parameter is never used
        {
            app.UseMvc();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvcCore().AddJsonFormatters();

            // Persistence
            if (Configuration["persistence:type"] == "ephemeral")
            {
                services.AddSingleton<IWorkQuotaRepository, MemoryWorkQuotaRepository>();
            }
            else if (Configuration["persistence:type"] == "mongodb")
            {
                services.AddSingleton<IWorkQuotaRepository, MongoWorkQuotaRepository>();
                services.Configure<MongoDbRepositorySettings>(Configuration.GetSection("persistence"));
            }

            // Injection
            services.AddScoped<IUserCuotaDeTrabajo, UserCuotaDeTrabajo>();
        }
    }
#pragma warning restore SA1600 // Elements must be documented
}