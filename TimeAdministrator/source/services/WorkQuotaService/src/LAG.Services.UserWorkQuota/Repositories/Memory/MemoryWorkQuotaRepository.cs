using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPY.Services.UserWorkQuota.Repositories.Models;

namespace EPY.Services.UserWorkQuota.Repositories.Memory
{
    /// <summary>
    /// In-Memory persistence of work quotas
    /// </summary>
    public class MemoryWorkQuotaRepository : IWorkQuotaRepository
    {
        /// <summary>
        /// Gets or sets a number of preexisting work quotas
        /// </summary>
        public Dictionary<string, WorkQuota> WorkQuotasById { get; set; } = new Dictionary<string, WorkQuota>();

        /// <inheritdoc/>
        public Task CreateWorkQuotaAsync(WorkQuota creation)
        {
            WorkQuotasById.Add(creation.UserId, creation);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task DeleteWorkQuotaAsync(string id)
        {
            WorkQuotasById.Remove(id);
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task<WorkQuota> ReadWorkQuotaByUserIdAsync(string id)
        {
            WorkQuota workQuota = WorkQuotasById.FirstOrDefault(g => g.Key == id).Value;
            return Task.FromResult(workQuota);
        }

        /// <inheritdoc/>
        public Task UpdateWorkQuotaAsync(WorkQuota update)
        {
            WorkQuotasById[update.UserId] = update;
            return Task.CompletedTask;
        }
    }
}
