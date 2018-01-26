using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Docugate.Services.Common.Client.Http
{
    /// <summary>
    /// Configuration delegate used when creating new clients
    /// </summary>
    /// <param name="client">the uninitialized <see cref="HttpClient"/></param>
    public delegate void ConfigureClient(HttpClient client);

    /// <summary>
    /// Handler for middlewares to call their successive middlewares
    /// </summary>
    /// <returns>an awaitable <see cref="Task"/> of the successor middleware</returns>
    public delegate Task<HttpResponseMessage> NextMiddleware();

    /// <summary>
    /// <see cref="HttpClient"/> request handling middleware
    /// </summary>
    /// <param name="request">the current request</param>
    /// <param name="cancellationToken">the cancellation token for when requests get cancelled by the caller</param>
    /// <param name="next">dispatches the current call to the next middleware</param>
    /// <returns>an waitable <see cref="Task"/> for the calling middleware to await</returns>
    public delegate Task<HttpResponseMessage> ClientMiddleware(HttpRequestMessage request, CancellationToken cancellationToken, NextMiddleware next);

    /// <summary>
    /// Represents a simplification of the more complex <see cref="ClientMiddleware" />.
    /// </summary>
    /// <param name="request">the current request</param>
    /// <param name="cancellationToken">the cancellation token for when requests get cancelled by the caller</param>
    /// <returns>an waitable <see cref="Task"/> for the calling middleware to await</returns>
    internal delegate Task<HttpResponseMessage> Middleware(HttpRequestMessage request, CancellationToken cancellationToken);

    /// <summary>
    /// Base class for http web API clients.
    /// </summary>
    public interface IHttpClientBase
    {
        /// <summary>
        /// Create a new <see cref="HttpClient"/> with the given <see cref="ClientMiddleware"/>s
        /// that have been added via <see cref="Use(ClientMiddleware)"/>
        /// </summary>
        /// <returns>the newly setup client</returns>
        HttpClient NewClient();

        /// <summary>
        /// Adds another middleware to the handler.
        /// </summary>
        /// <param name="middleware">The middleware handler</param>
        void Use(ClientMiddleware middleware);
    }
}
