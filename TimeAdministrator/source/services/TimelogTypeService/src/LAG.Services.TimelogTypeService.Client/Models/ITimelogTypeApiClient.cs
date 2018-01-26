using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using RestEase;

namespace EPY.Services.TipoLogTiempoService.Client.Models
{
    public interface ITipoLogTiempoApiClient
    {
        [Get("{TipoLogTiempoid}")]
        Task<TipoLogTiempoResponse> GetTipoLogTiempoById([Path] string TipoLogTiempoid);

        [Get("")]
        Task<IEnumerable<TipoLogTiempoResponse>> GetAllTipoLogTiempos();
    }
}
