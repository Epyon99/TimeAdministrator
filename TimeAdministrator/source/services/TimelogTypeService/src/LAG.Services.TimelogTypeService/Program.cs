using System.EventSourcing.AspNetCore.Hosting;
using System.EventSourcing.AspNetCore.Kafka;
using EPY.Services.TipoLogTiempoService.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace EPY.Services.TipoLogTiempoService
{
    public class Program
    {
        public static void Main(string[] args)
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
