using System;
using System.EventSourcing.AspNetCore.Hosting;
using System.EventSourcing.AspNetCore.Kafka;
using EPY.Services.TipoLogTiempoService.Configuration;
using EPY.Services.TipoLogTiempoService.Repositories;
using EPY.Services.TipoLogTiempoService.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EPY.Services.TipoLogTiempoService.Hosting
{
    public class StartupProjections
    {
        readonly IRedirectionTarget redirTgt;

        public StartupProjections(IRedirectionTarget redirTgt, IHostingEnvironment environment)
        {
            Environment = environment ?? throw new ArgumentNullException(nameof(environment));

            var builder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment.EnvironmentName.ToLower()}.json", optional: true);
            Configuration = builder.Build();

            this.redirTgt = redirTgt;
        }

        public IHostingEnvironment Environment { get; private set; }

        public IConfigurationRoot Configuration { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<IdentityServerAuthenticationOptions>(Configuration.GetSection("authentication"));

            services.UseKafka(
                    x =>
                    {
                        var kafkaSettings = Configuration.GetSection("kafka").Get<KafkaConfiguration>();
                        x.BootstrapServers = kafkaSettings.BootstrapServers;
                        x.Topics = kafkaSettings.Topics;
                        x.ConsumerGroup = kafkaSettings.ConsumerGroup;
                    });

            // Add framework services.
            services.AddMvcCore()
                .AddJsonFormatters();

            services.AddSingleton(x => redirTgt.Provider.GetService<ITipoLogTiempoRepository>());
            services.AddTransient(svc => redirTgt.Provider.GetService<ITipoLogTiempoService>());
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                loggerFactory.AddConsole();
                loggerFactory.AddDebug();
            }

            app.UseMvc();
        }
    }
}