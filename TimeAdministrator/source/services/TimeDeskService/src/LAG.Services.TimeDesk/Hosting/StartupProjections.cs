using System;
using System.EventSourcing.AspNetCore.Hosting;
using System.EventSourcing.AspNetCore.Hosting.Authorization;
using System.EventSourcing.AspNetCore.Kafka;
using Docugate.Services.Common.Client.Http;
using EPY.Services.LogTiempo.Hosting.Models;
using EPY.Services.LogTiempo.Repositories;
using EPY.Services.LogTiempo.Services;
using EPY.Services.TipoLogTiempoService.Client;
using EPY.Services.UserWorkQuota.Client;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace EPY.Services.LogTiempo
{
    public class StartupProjections
    {
        readonly IRedirectionTarget redirTgt;

        public StartupProjections(IRedirectionTarget redirTgt, IHostingEnvironment env)
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

            this.redirTgt = redirTgt;
        }

        public IConfigurationRoot Configuration { get; private set; }

        public IHostingEnvironment Environment { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddMvcCore()
                .AddJsonFormatters();

            services.UseKafka(
                    x =>
                    {
                        var kafkaSettings = Configuration.GetSection("kafka").Get<KafkaConfiguration>();
                        x.BootstrapServers = kafkaSettings.BootstrapServers;
                        x.Topics = kafkaSettings.Topics;
                        x.ConsumerGroup = kafkaSettings.ConsumerGroup;
                    });

            services.AddSingleton(x => redirTgt.Provider.GetService<ITimeLogRepository>());
            services.AddSingleton(x => redirTgt.Provider.GetService<IReasonsRepository>());
            services.AddTransient<ITimeLogService, TimeLogService>();
            services.AddTransient<ITimeSpanInfoService, TimeSpanInfoService>();
            services.AddTransient<IReasonsInfoService, ReasonsInfoService>();
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
            services.AddSingleton(svc => redirTgt.Provider.GetService<IOptions<ConfigurationOptions>>());
            services.AddSingleton(svc => redirTgt.Provider.GetService<IOptions<IdentityServerAuthenticationOptions>>());
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                loggerFactory.AddConsole();
                loggerFactory.AddDebug();
            }

            // Link in Authentication
            var option = app.ApplicationServices.GetService<IOptions<IdentityServerAuthenticationOptions>>();
            var authOptions = option.Value;
            app.UseImpersonationBearer(authOptions.Authority, authOptions.ApiName, authOptions.ApiSecret);

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