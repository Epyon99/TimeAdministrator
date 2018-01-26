using System;
using System.Threading.Tasks;
using EPY.Services.Common.Service.Models;
using EPY.Services.UserWorkQuota.Repositories;
using EPY.Services.UserWorkQuota.Repositories.Models;

namespace EPY.Services.UserWorkQuota.Services
{
    /// <summary>
    /// Implements <see cref="IUserCuotaDeTrabajo"/>
    /// </summary>
    public class UserCuotaDeTrabajo : IUserCuotaDeTrabajo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserCuotaDeTrabajo"/> class.
        /// </summary>
        /// <param name="repo">underlaying <see cref="IWorkQuotaRepository"/></param>
        public UserCuotaDeTrabajo(IWorkQuotaRepository repo)
        {
            UserWorkQuotaRepository = repo;
        }

        IWorkQuotaRepository UserWorkQuotaRepository { get; set; }

        /// <inheritdoc/>
        public async Task<ServiceResult<WorkQuota>> AddUserWorkQuota(string userid, string workquota)
        {
            var newWorkQuota = new WorkQuota
            {
                Id = Guid.NewGuid().ToString(),
                UserId = userid,
                DailyWorkQuota = workquota
            };
            try
            {
                await UserWorkQuotaRepository.CreateWorkQuotaAsync(newWorkQuota);

                return new ServiceResult<WorkQuota>
                {
                    Status = Results.Success,
                    Result = newWorkQuota
                };
            }
            catch (Exception e)
            {
                return new ServiceResult<WorkQuota>
                {
                    Status = Results.Failed,
                    Error = e
                };
            }
        }

        /// <inheritdoc/>
        public async Task<ServiceResult> DeleteUserWorkQuota(string userid)
        {
            try
            {
                await UserWorkQuotaRepository.DeleteWorkQuotaAsync(userid);

                return new ServiceResult
                {
                    Status = Results.Success,
                    Result = "Success"
                };
            }
            catch (Exception e)
            {
                return new ServiceResult
                {
                    Status = Results.Failed,
                    Error = e
                };
            }
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<WorkQuota>> GetUserWorkQuotaByUserId(string userid)
        {
            try
            {
                var data = await UserWorkQuotaRepository.ReadWorkQuotaByUserIdAsync(userid);
                if (data == null)
                {
                    return new ServiceResult<WorkQuota>
                    {
                        Status = Results.NotFound,
                        Result = data
                    };
                }

                return new ServiceResult<WorkQuota>
                {
                    Status = Results.Success,
                    Result = data
                };
            }
            catch (Exception e)
            {
                return new ServiceResult<WorkQuota>
                {
                    Status = Results.Failed,
                    Error = e
                };
            }
        }

        /// <inheritdoc/>
        public async Task<ServiceResult<WorkQuota>> UpdateUserWorkQuota(string userid, string workquota)
        {
            try
            {
                var userWorkQuota = await GetUserWorkQuotaByUserId(userid);

                if (userWorkQuota == null)
                {
                    return new ServiceResult<WorkQuota>
                    {
                        Status = Results.NotFound
                    };
                }

                userWorkQuota.Result.DailyWorkQuota = workquota;
                await UserWorkQuotaRepository.UpdateWorkQuotaAsync(userWorkQuota.Result);

                return new ServiceResult<WorkQuota>
                {
                    Status = Results.Success,
                    Result = userWorkQuota.Result
                };
            }
            catch (Exception e)
            {
                return new ServiceResult<WorkQuota>
                {
                    Status = Results.Failed,
                    Error = e
                };
            }
        }
    }
}
