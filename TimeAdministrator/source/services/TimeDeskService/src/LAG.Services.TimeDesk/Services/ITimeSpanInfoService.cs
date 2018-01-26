using System;
using System.Threading.Tasks;
using EPY.Services.LogTiempo.Repositories.Models;

namespace EPY.Services.LogTiempo.Repositories
{
    public interface ITimeSpanInfoService
    {
        Task<LogTimeSpan> GetLastTimeSpanForUser(string userid);

        LogTimeSpan AddLogToTimeSpan(LogTimeSpan currentTimeSpan, TimeLog newTimeLog);

        Task<LogTimeSpan> CalculateEndTimeWithType(string id, DateTimeOffset createDate, string type, string user);
    }
}
