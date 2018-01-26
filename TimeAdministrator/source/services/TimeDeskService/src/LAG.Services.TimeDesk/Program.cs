using System.EventSourcing.AspNetCore.Hosting;
using System.EventSourcing.AspNetCore.Kafka;
using Microsoft.AspNetCore.Hosting;

namespace EPY.Services.LogTiempo
{
    public class Program
    {
        public static void Main()
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseStartup<Startup>()
                .Build();

            var projectionsHost = new WebHostBuilder()
                .EnableRedirection(host.Services)
                .UseStartup<StartupProjections>()
                .Build();

            new[] { host, projectionsHost }.Run();
        }
    }
}