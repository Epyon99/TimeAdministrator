using Docugate.Services.Common.Client.Http;
using RestEase;

namespace Docugate.Services.Common.Client.RestEase
{
    public class RestEaseClientBase : HttpClientBase
    {
        public RestEaseClientBase(string baseAddress)
            : base(baseAddress)
        {
        }

        protected T GetClient<T>()
        {
            return RestClient.For<T>(NewClient());
        }
    }
}
