using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Docugate.Services.Common.Client.Http
{
    /// <summary>
    /// The <see cref="DelegatingHttpHandler"/> handles HTTP request by first passing them through
    /// a series of Middlewares
    /// </summary>
    class DelegatingHttpHandler : HttpClientHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DelegatingHttpHandler"/> class with the last middleware dispatching
        /// to the <see cref="HttpClientHandler.SendAsync(HttpRequestMessage, CancellationToken)"/> method.
        /// </summary>
        public DelegatingHttpHandler()
        {
            Middleware = (req, token) => { return base.SendAsync(req, token); };
        }

        Middleware Middleware { get; set; }

        /// <summary>
        /// Adds middleware handler that will be executed when calling <see cref="SendAsync(HttpRequestMessage, CancellationToken)"/>
        /// </summary>
        /// <param name="middleware">the Middelware definitions</param>
        public void Use(ClientMiddleware middleware)
        {
            var successorMiddleware = Middleware;
            Middleware = (req, token) => middleware(req, token, () => successorMiddleware(req, token));
        }

        /// <inheritdoc/>
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Middleware(request, cancellationToken);
        }
    }
}
