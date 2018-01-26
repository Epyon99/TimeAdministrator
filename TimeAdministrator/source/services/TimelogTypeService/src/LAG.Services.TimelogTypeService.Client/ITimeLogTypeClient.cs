using System.Collections.Generic;
using System.Threading.Tasks;
using EPY.Services.TipoLogTiempoService.Client.Models;

namespace EPY.Services.TipoLogTiempoService.Client
{
    public interface ITipoLogTiempoClient
    {
        Task<IEnumerable<TipoLogTiempoResponse>> GetAllTipoLogTiempos();

        Task<TipoLogTiempoResponse> GetTipoLogTiempoById(string id);
    }
}
