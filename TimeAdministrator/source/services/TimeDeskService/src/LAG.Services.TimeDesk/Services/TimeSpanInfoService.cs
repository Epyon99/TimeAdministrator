using System;
using System.Globalization;
using System.Threading.Tasks;
using EPY.Services.LogTiempo.Hosting.Models;
using EPY.Services.LogTiempo.Repositories.Models;
using EPY.Services.TipoLogTiempoService.Client;
using EPY.Services.TipoLogTiempoService.Client.Models;
using EPY.Services.UserWorkQuota.Client;
using EPY.Services.UserWorkQuota.Client.Models;
using Microsoft.Extensions.Options;
using RepoModels = EPY.Services.LogTiempo.Repositories.Models;

namespace EPY.Services.LogTiempo.Repositories
{
    public class TimeSpanInfoService : ITimeSpanInfoService
    {
        public const string DateTimeFormatPrecise = "yyyy-MM-ddTHH:mm:ssZ";
        private const float DayOfWork = 8.5f;

        private ITimeLogRepository repository;
        private IOptions<ConfigurationOptions> configurationOpt;

        public TimeSpanInfoService(ITimeLogRepository repository, IOptions<ConfigurationOptions> optionsAccessor, ITipoLogTiempoClient TipoLogTiempoClient, IUserWorkQuotaClient userWorkQuotaClient)
        {
            this.repository = repository;
            configurationOpt = optionsAccessor;
            TipoLogTiempoClient = TipoLogTiempoClient;
            UserWorkQuotaClient = userWorkQuotaClient;
        }

        public IUserWorkQuotaClient UserWorkQuotaClient { get; private set; }

        public ITipoLogTiempoClient TipoLogTiempoClient { get; private set; }

        public LogTimeSpan AddLogToTimeSpan(LogTimeSpan currentTimeSpan, TimeLog newTimeLog)
        {
            if (currentTimeSpan != null)
            {
                currentTimeSpan.EndTime = newTimeLog.Time;
                return currentTimeSpan;
            }

            return new LogTimeSpan
            {
                Id = newTimeLog.Id.ToString().ToLower(),
                StartTime = newTimeLog.Time
            };
        }

        public async Task<LogTimeSpan> GetLastTimeSpanForUser(string userid)
        {
            TimeLogSpan timeSpan = await repository.GetLastTimeSpanAsync(userid);
            if (timeSpan != null)
            {
                return new LogTimeSpan
                {
                    Id = timeSpan.Id,
                    StartTime = DateTimeOffset.Parse(timeSpan.TimeStart),
                    StartReason = timeSpan.TimeStartReason
                };
            }

            return null;
        }

        public async Task<LogTimeSpan> CalculateEndTimeWithType(string id, DateTimeOffset date, string type, string user)
        {
            var ttRequest = await TipoLogTiempoClient.GetTipoLogTiempoById(type);
            if (ttRequest == null)
            {
                ttRequest = new TipoLogTiempoResponse { Name = "work", Color = "blue" };
            }

            var TipoLogTiempo = new TipoLogTiempo() { Color = ttRequest.Color, Factor = ttRequest.Factor, Id = ttRequest.Id, Name = ttRequest.Name };
            var timeStart = date;
            if (TipoLogTiempo.Name.ToLower().Contains("work"))
            {
                var timeDate = new DateTime(timeStart.Date.Year, timeStart.Date.Month, timeStart.Date.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
                timeStart = new DateTimeOffset(timeDate).ToUniversalTime();
            }

            var userWorkQuota = await UserWorkQuotaClient.GetUserWorkQuota(user);
            if (userWorkQuota == null)
            {
                userWorkQuota = new UserWorkQuotaResponse() { UserDailyWorkQuota = string.Empty };
            }

            var dailyQuota = 0f;
            if (!string.IsNullOrEmpty(userWorkQuota.UserDailyWorkQuota))
            {
                dailyQuota = float.Parse(userWorkQuota.UserDailyWorkQuota) / 100;
            }
            else
            {
                dailyQuota = 1;
            }

            // this means DayOfWork is a const of 8.5hours.
            // So the product of DayOfWork*dailyWorkQuota, represent it's full day of work
            // And from that quantity times the factor would be the number of hours that an especial
            // event covers.
            // Day of work 8.5 hours. Daily Work Quota 50%. A day of 4.25hours
            // times the factor of a Half Day holiday would be 2.125 hours
            var tempFactor = (TipoLogTiempo.Factor == 0) ? 1 : TipoLogTiempo.Factor;
            var timeEnd = timeStart
                .Add(new TimeSpan((int)((DayOfWork * dailyQuota) * tempFactor), (int)((DayOfWork % dailyQuota) * 60), 0));

            return new LogTimeSpan
            {
                Id = id.ToString(),
                StartTime = timeStart,
                EndTime = timeEnd,
            };
        }
    }
}
