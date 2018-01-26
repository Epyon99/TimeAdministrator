using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPY.Services.LogTiempo.Repositories.Models;

namespace EPY.Services.LogTiempo.Repositories.Memory
{
    public class MemoryTimeLogRepository : ITimeLogRepository
    {
        public Dictionary<string, TimeLogSpan> TimeLogsById { get; set; } = new Dictionary<string, TimeLogSpan>();

        public Task CreateTimeLog(TimeLogSpan creation)
        {
            TimeLogsById.Add(creation.Id, creation);
            return Task.CompletedTask;
        }

        public Task DeleteTimeLog(string id)
        {
            TimeLogsById.Remove(id);
            return Task.CompletedTask;
        }

        public Task<TimeLogSpan> GetLastTimeSpanAsync(string user)
        {
            var openTimeLogs = TimeLogsById.Where(g => g.Value.UserId == user && string.IsNullOrEmpty(g.Value.TimeEnd));
            if (openTimeLogs.Any())
            {
                return Task.FromResult(openTimeLogs.FirstOrDefault().Value);
            }

            return Task.FromResult<TimeLogSpan>(null);
        }

        public Task<IEnumerable<TimeLogSpan>> ReadLogsByDateRange(string user, DateTime from, DateTime till)
        {
            return Task.Run(() => TimeLogsById.Where(g => g.Value.UserId == user
                                                        && (DateTime.Parse(g.Value.TimeStart) >= from)
                                                        && (DateTime.Parse(g.Value.TimeStart) < till))
                                              .Select(g => g.Value));
        }

        public Task<TimeLogSpan> ReadTimeLogById(string logId)
        {
            return Task.FromResult(TimeLogsById[logId]);
        }

        public Task UpdateTimelog(TimeLogSpan update)
        {
            TimeLogsById[update.Id] = update;
            return Task.CompletedTask;
        }
    }
}