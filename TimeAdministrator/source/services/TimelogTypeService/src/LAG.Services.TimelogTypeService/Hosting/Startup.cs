using System;
using System.EventSourcing.AspNetCore.Hosting;
using System.EventSourcing.Client;
using System.EventSourcing.Client.Kafka;
using System.EventSourcing.Client.Reflection;
using System.EventSourcing.Client.Serialization;
using EPY.Services.TipoLogTiempoService.Repositories;
using EPY.Services.TipoLogTiempoService.Repositories.Memory;
using EPY.Services.TipoLogTiempoService.Repositories.MongoDb;
using EPY.Services.TipoLogTiempoService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EPY.Services.TipoLogTiempoService.Hosting
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

            services.AddTransient<ITipoLogTiempoService, TipoLogTiempoService>();

            services.AddScoped<IEventClient>(x => new EventClient()
                .UseKafka("system.events", "epyhost:9092")
                .UseReflectionNameResolution()
                .UseJsonSerialization());

            // read settings for authentication middleware
            services.Configure<IdentityServerAuthenticationOptions>(Configuration.GetSection("authentication"));

            // Initialize persistence
            if (Configuration["persistence:type"] == "ephemeral")
            {
                services.AddSingleton<ITipoLogTiempoRepository, MemoryTipoLogTiempoRepository>();
            }
            else if (Configuration["persistence:type"] == "mongodb")
            {
                services.AddSingleton<ITipoLogTiempoRepository, MongoTipoLogTiempoRepository>();
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
                ApiName = authOptions.ApiName,
                ApiSecret = authOptions.ApiSecret
            });

            app.UseMvc();
        }
    }
}