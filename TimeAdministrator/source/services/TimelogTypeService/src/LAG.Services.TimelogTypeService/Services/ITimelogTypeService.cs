using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EPY.Services.Common.Service.Models;
using EPY.Services.TipoLogTiempoService.Services.Models;

namespace EPY.Services.TipoLogTiempoService.Services
{
    public interface ITipoLogTiempoService
    {
        /// <summary>
        /// Get all the TipoLogTiempos
        /// </summary>
        /// <returns>A collection with all the TipoLogTiempos</returns>
        Task<ServiceResult<IEnumerable<TipoLogTiempo>>> GetAllTipoLogTiempos();

        /// <summary>
        /// Find one TipoLogTiempo by Id
        /// </summary>
        /// <param name="id">The ID of the selected TipoLogTiempo</param>
        /// <returns>A TipoLogTiempo</returns>
        Task<ServiceResult<TipoLogTiempo>> GetTipoLogTiempoById(string id);

        /// <summary>
        /// Creates a new TipoLogTiempo
        /// </summary>
        /// <param name="color">Color to be represented #Hex value</param>
        /// <param name="factor">Factor of hours per day</param>
        /// <param name="name">Display name of the Type</param>
        /// <returns>Sucess message</returns>
        Task<ServiceResult> AddTipoLogTiempo(string color, float factor, string name);

        /// <summary>
        /// Updates an existing TipoLogTiempo
        /// </summary>
        /// <param name="id">The ID of the existing TipoLogTiempo</param>
        /// <param name="color">RGB color</param>
        /// <param name="factor">Multiple Factor for the TipoLogTiempo</param>
        /// <param name="name">Display name of the Type</param>
        /// <returns>Sucess message, Not found if ID does not exist.</returns>
        Task<ServiceResult<TipoLogTiempo>> UpdateTipoLogTiempo(Guid id, string color, float factor, string name);

        /// <summary>
        /// Deletes an TipoLogTiempo by it's id
        /// </summary>
        /// <param name="id">The id to filter</param>
        /// <returns>Sucess message</returns>
        Task<ServiceResult> DeleteTipoLogTiempo(string id);
    }
}
