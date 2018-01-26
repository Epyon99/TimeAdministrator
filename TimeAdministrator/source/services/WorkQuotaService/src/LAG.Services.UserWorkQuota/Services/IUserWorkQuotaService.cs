using System.Threading.Tasks;
using EPY.Services.Common.Service.Models;
using EPY.Services.UserWorkQuota.Repositories.Models;

namespace EPY.Services.UserWorkQuota.Services
{
    /// <summary>
    /// Business service for the user work quota
    /// </summary>
    public interface IUserCuotaDeTrabajo
    {
        /// <summary>
        /// Adds a WorkQuota for an user
        /// </summary>
        /// <param name="userid">The user id </param>
        /// <param name="workquota">The workquota</param>
        /// <returns>A result object indicating Success if the quota was successfully created and the created quota, Failed if there was an error</returns>
        Task<ServiceResult<WorkQuota>> AddUserWorkQuota(string userid, string workquota);

        /// <summary>
        /// Deletes a Workquota.
        /// </summary>
        /// <param name="userid">The user id used for validation</param>
        /// <returns>A result object indicating Success if the quota was successfully deleted, Failed if there was an error</returns>
        Task<ServiceResult> DeleteUserWorkQuota(string userid);

        /// <summary>
        /// Updates a Workquota
        /// </summary>
        /// <param name="userid">The user id</param>
        /// <param name="workquota">The new value</param>
        /// <returns>A result object indicating Success if the quota was successfully updated and the updated quota, Failed if there was an error</returns>
        Task<ServiceResult<WorkQuota>> UpdateUserWorkQuota(string userid, string workquota);

        /// <summary>
        /// Gets only a Workquota for the user id
        /// </summary>
        /// <param name="userid">The user id for validation</param>
        /// <returns>A result object indicating Success and containing the WorkQuota, Failed if there was an error</returns>
        Task<ServiceResult<WorkQuota>> GetUserWorkQuotaByUserId(string userid);
    }
}
