using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Docugate.Services.Common.Client.RestEase;
using EPY.Services.TipoLogTiempoService.Client.Models;
using RestEase;

namespace EPY.Services.TipoLogTiempoService.Client
{
    public class TipoLogTiempoClient : RestEaseClientBase, ITipoLogTiempoClient
    {
        public TipoLogTiempoClient()
            : base("http://epyhost:5010/v1/types/")
        {
        }

        public TipoLogTiempoClient(string baseAddress)
            : base(baseAddress)
        {
        }

        public async Task<IEnumerable<TipoLogTiempoResponse>> GetAllTipoLogTiempos()
        {
            var client = GetClient<ITipoLogTiempoApiClient>();
            return await client.GetAllTipoLogTiempos();
        }

        public async Task<TipoLogTiempoResponse> GetTipoLogTiempoById(string id)
        {
            TipoLogTiempoResponse response = new TipoLogTiempoResponse();
            try
            {
                var client = GetClient<ITipoLogTiempoApiClient>();
                response = await client.GetTipoLogTiempoById(id);
            }
            catch (ApiException apiEx)
            {
                if (apiEx.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
            }

            return response;
        }

        public void UseAuthenticationHeader()
        {
            throw new NotImplementedException();
        }
    }
}
