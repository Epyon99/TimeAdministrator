using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EPY.Services.Common.Service.Models;
using EPY.Services.LogTiempo.Repositories.Models;
using EPY.Services.LogTiempo.Services.Models;

namespace EPY.Services.LogTiempo.Repositories
{
    public interface ITimeLogService
    {
        /// <summary>
        /// Creates a new timelog.
        /// </summary>
        /// <param name="timelog">Contains the userId, TipoLogTiempo name </param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task<ServiceResult<LogTimeSpan>> CreateTimeLogNow(TimeLogRequest timelog);

        Task<ServiceResult<IEnumerable<LogTimeSpan>>> CreateMultipleBookings(TimelogBookingsRequest booking);

        Task<ServiceResult<LogTimeSpan>> CreateTimeLogAtDate(TimeLogRequest timelog);

        /// <summary>
        /// Deletes a dtimelog.
        /// </summary>
        /// <param name="userid">The timelog user id used for validation</param>
        /// <param name="identifier">The timelog identifier</param>
        /// <returns>A result object containing detailed information.</returns>
        Task<ServiceResult> DeleteTimeLog(string userid, string identifier);

        /// <summary>
        /// Updates a timespawn already created.
        /// </summary>
        /// <param name="logId">Id of the Timespan stored</param>
        /// <param name="userId">UserId of the Timespan stored</param>
        /// <param name="timeStart">New TimeStart value</param>
        /// <param name="timeEnd">New TimeEnd Value</param>
        /// <param name="timeEndReason">New reason for the endTime update </param>
        /// <param name="timeStartReason">New reason for the startTime update</param>
        /// <returns>Updated values</returns>
        Task<ServiceResult<TimeLogUpdate>> UpdateTimeLog(string logId, string userId, string timeStart, string timeEnd, string timeEndReason, string timeStartReason);

        /// <summary>
        /// Get a collection of timelogs based on user and time range.
        /// </summary>
        /// <param name="userid">The user id </param>
        /// <param name="from">Sets since what date should timelogs be retrieved</param>
        /// <param name="till">Sets until what date should timelogs be retrieved (Optional - if not provided, a default offset will be set by the service).</param>
        /// <returns>A GetTimeLogsResult object</returns>
        Task<ServiceResult<IEnumerable<LogTimeSpan>>> GetTimeLogsByDaysRange(string userid, DateTimeOffset from, DateTimeOffset? till);
    }
}