using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EPY.Services.LogTiempo.Repositories.Models;

namespace EPY.Services.LogTiempo.Repositories
{
    public interface ITimeLogRepository
    {
        Task CreateTimeLog(TimeLogSpan creation);

        Task<TimeLogSpan> GetLastTimeSpanAsync(string user);

        Task UpdateTimelog(TimeLogSpan update);

        Task DeleteTimeLog(string id);

        Task<TimeLogSpan> ReadTimeLogById(string logId);

        Task<IEnumerable<TimeLogSpan>> ReadLogsByDateRange(string user, DateTime from, DateTime till);
    }
}