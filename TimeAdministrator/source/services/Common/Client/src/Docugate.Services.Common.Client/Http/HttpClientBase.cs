using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Docugate.Services.Common.Client.Http
{
    public class HttpClientBase : IHttpClientBase
    {
        public HttpClientBase(string baseAddress)
        {
            ClientConfigurators.Add((client) => client.BaseAddress = new Uri(baseAddress));
            Handler = new DelegatingHttpHandler();
        }

        public List<ConfigureClient> ClientConfigurators { get; private set; } = new List<ConfigureClient>();

        DelegatingHttpHandler Handler { get; set; }

        public HttpClient NewClient()
        {
            var client = new HttpClient(Handler);

            foreach (var configurator in ClientConfigurators)
            {
                configurator(client);
            }

            return client;
        }

        public void Use(ClientMiddleware middleware)
        {
            Handler.Use(middleware);
        }
    }
}
