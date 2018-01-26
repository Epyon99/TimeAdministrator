using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EPY.Services.LogTiempo.Repositories.Models;

namespace EPY.Services.LogTiempo.Repositories.Memory
{
    public class MemoryReasonsRepository : IReasonsRepository
    {
        public Dictionary<string, List<Reasons>> ReasonsByUserId { get; set; } = new Dictionary<string, List<Reasons>>();

        public Task AddUserReason(UserReasons userReason)
        {
            ReasonsByUserId.Add(userReason.UserId, userReason.Reasons);
            return Task.CompletedTask;
        }

        public Task<UserReasons> GetAllUserReasons(string user)
        {
            return Task.FromResult(new UserReasons()
            {
                Reasons = ReasonsByUserId[user].OrderByDescending(g => g.Weight).ToList(),
                UserId = user,
        });
        }

        public Task<UserReasons> GetTopUserReasons(string user)
        {
            return Task.FromResult(new UserReasons()
            {
                Reasons = ReasonsByUserId[user].OrderByDescending(g => g.Weight).Take(10).ToList(),
                UserId = user,
            });
        }

        public Task UpdateUserReason(UserReasons userReason)
        {
            ReasonsByUserId[userReason.UserId] = userReason.Reasons;
            return Task.CompletedTask;
        }
    }
}
