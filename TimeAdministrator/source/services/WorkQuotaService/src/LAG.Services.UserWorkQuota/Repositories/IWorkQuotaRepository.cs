using System.Threading.Tasks;
using EPY.Services.UserWorkQuota.Repositories.Models;

namespace EPY.Services.UserWorkQuota.Repositories
{
    /// <summary>
    /// Reads and writes work quotas to persistence
    /// </summary>
    public interface IWorkQuotaRepository
    {
        /// <summary>
        /// Create a new quota
        /// </summary>
        /// <param name="creation">new work quota record</param>
        /// <returns>Async task</returns>
        Task CreateWorkQuotaAsync(WorkQuota creation);

        /// <summary>
        /// Updates an existing quota
        /// </summary>
        /// <param name="update">the updated quota</param>
        /// <returns>Async task</returns>
        Task UpdateWorkQuotaAsync(WorkQuota update);

        /// <summary>
        /// Deletes a work quota
        /// </summary>
        /// <param name="userid">The id of the work quota</param>
        /// <returns>Async task</returns>
        Task DeleteWorkQuotaAsync(string userid);

        /// <summary>
        /// Returns a quota by the users id
        /// </summary>
        /// <param name="userid">the users id</param>
        /// <returns>Async task for a <see cref="WorkQuota"/></returns>
        Task<WorkQuota> ReadWorkQuotaByUserIdAsync(string userid);
    }
}
