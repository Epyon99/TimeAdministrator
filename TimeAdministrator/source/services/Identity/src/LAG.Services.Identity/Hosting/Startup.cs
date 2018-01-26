using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Services.InMemory;
using IdentityServer4.Validation;
using EPY.Services.Identity.Infrastructure;
using EPY.Services.Identity.Infrastructure.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace EPY.Services.Identity.Hosting
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            if (env == null)
            {
                throw new ArgumentNullException(nameof(env));
            }

            this.Environment = env;

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);
            this.Configuration = builder.Build();
        }

        public int IOption { get; private set; }
        private IConfigurationRoot Configuration { get; set; }
        private IHostingEnvironment Environment { get; set; }

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

            app.UseIdentityServer();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //  map strong typed settings
            services.Configure<ActiveDirectorySettings>(this.Configuration.GetSection(nameof(ActiveDirectorySettings)));

            services.AddIdentityServer()
                .AddInMemoryClients(Clients.Get())
                .AddInMemoryScopes(Scopes.Get());

            services.AddTransient<IResourceOwnerPasswordValidator, ActiveDirectoryOwnerPasswordValidator>();
            services.AddTransient<IProfileService, ActiveDirectoryProfileService>();
            services.AddTransient<IActiveDirectoryAuthenticationManager, IActiveDirectoryAuthenticationManager>(o => new ActiveDirectoryAuthenticationManager(o.GetRequiredService<IOptions<ActiveDirectorySettings>>().Value.Server));

            // Add framework services.
            services.AddMvc();
        }
    }

    internal class Clients
    {
        public static IEnumerable<Client> Get()
        {
            return new List<Client> {
                new Client {
                    ClientId = "mvcClient",
                    ClientName = "Example Client Credentials Client Application",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    ClientSecrets = new List<Secret> {
                        new Secret("mvcClientPassword".Sha256())},
                    AllowedScopes = new List<string> {"customAPI"}
                }
            };
        }
    }

    internal class Scopes
    {
        public static IEnumerable<Scope> Get()
        {
            return new List<Scope> {
                StandardScopes.OpenId,
                StandardScopes.Profile,
                StandardScopes.Email,
                StandardScopes.Roles,
                StandardScopes.OfflineAccess,
                new Scope {
                    Name = "customAPI",
                    DisplayName = "Custom API",
                    Description = "Custom API scope",
                    Type = ScopeType.Resource,
                    Claims = new List<ScopeClaim> {
                        new ScopeClaim(JwtClaimTypes.Role)
                    },
                    ScopeSecrets =  new List<Secret> {
                        new Secret("scopeSecret".Sha256())
                    }
                    ,
                    IncludeAllClaimsForUser=true,
                }
            };
        }
    }
}