using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPY.Services.Common.Service.Models;
using EPY.Services.LogTiempo.Repositories;
using EPY.Services.LogTiempo.Services.Models;
using Repo = EPY.Services.LogTiempo.Repositories.Models;

namespace EPY.Services.LogTiempo.Services
{
    public class ReasonsInfoService : IReasonsInfoService
    {
        private IReasonsRepository repository;

        public ReasonsInfoService(IReasonsRepository repository)
        {
            this.repository = repository;
        }

        public async Task<ServiceResult> AddReasonForUser(string keywords, string user)
        {
            try
            {
                keywords = keywords.ToLower();
                var reason = new Repo.Reasons() { Keyword = keywords, Weight = 0 };

                // find if the words exist for the user.
                var userReasons = await repository.GetAllUserReasons(user);

                // If there is any word
                if (userReasons != null && userReasons.Reasons != null && userReasons.Reasons.Count > 0)
                {
                    // Look for it and sum
                    var r = userReasons.Reasons.Where(g => g.Keyword == keywords).FirstOrDefault();
                    if (r != null)
                    {
                        r.Weight++;
                    }
                    else
                    {
                        // or add it.
                        userReasons.Reasons.Add(reason);
                    }

                    await repository.UpdateUserReason(userReasons);
                }
                else
                {
                    if (userReasons == null)
                    {
                        userReasons = new Repo.UserReasons();
                    }

                    if (userReasons.Reasons == null)
                    {
                        userReasons.Reasons = new List<Repo.Reasons>();
                    }

                    // add a new User and Word
                    userReasons.Reasons.Add(new Repo.Reasons() { Keyword = keywords, Weight = 0 });
                    await repository.AddUserReason(new Repo.UserReasons()
                    {
                        Reasons = userReasons.Reasons,
                        UserId = user,
                    });
                }

                return new ServiceResult
                {
                    Status = Results.Success,
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

        public async Task<ServiceResult<IEnumerable<string>>> GetReasonsForUser(string keywords, string user)
        {
            try
            {
                var logTimeSpan = await repository.GetTopUserReasons(user);

                if (logTimeSpan == null)
                {
                    return new ServiceResult<IEnumerable<string>>
                    {
                        Status = Results.Success,
                        Result = new List<string>(),
                    };
                }

                return new ServiceResult<IEnumerable<string>>
                {
                    Status = Results.Success,
                    Result = logTimeSpan.Reasons.Select(g => g.Keyword),
                };
            }
            catch (Exception e)
            {
                return new ServiceResult<IEnumerable<string>>
                {
                    Status = Results.Failed,
                    Error = e
                };
            }
        }
    }
}
