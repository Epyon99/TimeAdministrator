using System;
using System.EventSourcing.AspNetCore.Hosting;
using System.EventSourcing.Client;
using System.EventSourcing.Client.Authorization;
using System.EventSourcing.Client.Kafka;
using System.EventSourcing.Client.Reflection;
using System.EventSourcing.Client.Serialization;
using Docugate.Services.Common.Client.Http;
using EPY.Services.LogTiempo.Hosting.Models;
using EPY.Services.LogTiempo.Repositories;
using EPY.Services.LogTiempo.Repositories.Memory;
using EPY.Services.LogTiempo.Repositories.MongoDb;
using EPY.Services.LogTiempo.Services;
using EPY.Services.TipoLogTiempoService.Client;
using EPY.Services.UserWorkQuota.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EPY.Services.LogTiempo
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            Environment = env ?? throw new ArgumentNullException(nameof(env));

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName.ToLower()}.json", optional: true);
            Configuration = builder.Build();
        }

        IConfigurationRoot Configuration { get; set; }

        IHostingEnvironment Environment { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvcCore()
                .AddAuthorization()
                .AddJsonFormatters();

            services.AddTransient<ITimeLogService, TimeLogService>();
            services.AddTransient<ITimeSpanInfoService, TimeSpanInfoService>();
            services.AddTransient<IReasonsInfoService, ReasonsInfoService>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<ITipoLogTiempoClient>(x =>
            {
                string workbenchBaseAddress = Configuration.GetSection("services").GetSection("TipoLogTiempoUrl").Value;
                var client = new TipoLogTiempoClient(workbenchBaseAddress);
                client.UseRequestAuthorizationForwarding(x);
                return client;
            });

            services.AddScoped<IUserWorkQuotaClient>(x =>
            {
                string workbenchBaseAddress = Configuration.GetSection("services").GetSection("UserWorkQuotaUrl").Value;
                var client = new UserWorkQuotaClient(workbenchBaseAddress);
                client.UseRequestAuthorizationForwarding(x);
                return client;
            });

            services.AddScoped<IEventClient>(x => new EventClient()
                .UseKafka("system.events", "epyhost:9092")
                .UseReflectionNameResolution()
                .UseJsonSerialization()
                .UseAuthorizationForwarding(x));

            // read settings for authentication middleware
            services.Configure<IdentityServerAuthenticationOptions>(Configuration.GetSection("authentication"));

            services.Configure<ConfigurationOptions>(Configuration.GetSection("services"));

            // Initialize persistence
            if (Configuration["persistence:type"] == "ephemeral")
            {
                services.AddSingleton<ITimeLogRepository, MemoryTimeLogRepository>();
                services.AddSingleton<IReasonsRepository, MemoryReasonsRepository>();
            }
            else if (Configuration["persistence:type"] == "mongodb")
            {
                services.AddSingleton<ITimeLogRepository, MongoTimeLogRepository>();
                services.AddSingleton<IReasonsRepository, MongoReasonsRepository>();
                services.Configure<MongoDbRepositorySettings>(Configuration.GetSection("persistence"));
            }
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                loggerFactory.AddConsole();
                loggerFactory.AddDebug();
            }

            if (env.IsProduction() || env.IsStaging())
            {
                // Enable Metrics
            }

            // Deny request to /v1/evnets/*
            app.DenyEventSourcing();

            // obtain strong typed settings
            var option = app.ApplicationServices.GetService<IOptions<IdentityServerAuthenticationOptions>>();
            var authOptions = option.Value;

            app.UseIdentityServerAuthentication(new IdentityServerAuthenticationOptions
            {
                Authority = authOptions.Authority,
                RequireHttpsMetadata = authOptions.RequireHttpsMetadata,
                EnableCaching = authOptions.EnableCaching,
                ApiName = authOptions.ApiName,
                ApiSecret = authOptions.ApiSecret
            });

            app.UseMvc();
        }
    }
}