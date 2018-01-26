using System.Collections.Generic;
using System.Threading.Tasks;
using EPY.Services.TipoLogTiempoService.Repositories.Models;

namespace EPY.Services.TipoLogTiempoService.Repositories
{
    public interface ITipoLogTiempoRepository
    {
        /// <summary>
        /// Add a new Timelog Type to the collection.
        /// </summary>
        /// <param name="type">a TipoLogTiempo with Color, Id, Factor and Name</param>
        /// <returns>Sucess Task</returns>
        Task AddTipoLogTiempo(TipoLogTiempo type);

        /// <summary>
        /// Filters and return an TipoLogTiempo by it's ID
        /// </summary>
        /// <param name="id">The id to filter</param>
        /// <returns>Filtered TipoLogTiempo</returns>
        Task<TipoLogTiempo> GetTimelogById(string id);

        /// <summary>
        /// Get All TipoLogTiempos
        /// </summary>
        /// <returns>A collection of TipoLogTiempos</returns>
        Task<IEnumerable<TipoLogTiempo>> GetAllTimelogsTypes();

        /// <summary>
        /// Updates an existing typelogRequest
        /// </summary>
        /// <param name="type">The timelog type values to update</param>
        /// <returns>Sucess task.</returns>
        Task UpdateTipoLogTiempo(TipoLogTiempo type);

        /// <summary>
        /// Deletes a timelog Type
        /// </summary>
        /// <param name="id">Id to filter</param>
        /// <returns>Sucess task</returns>
        Task DeleteTipoLogTiempo(string id);
    }
}
