using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Docugate.Services.Common.Client.Http;
using EPY.Services.LogTiempo.Hosting.Models;
using EPY.Services.LogTiempo.Projections.Events;
using EPY.Services.LogTiempo.Repositories;
using EPY.Services.LogTiempo.Repositories.Models;
using EPY.Services.LogTiempo.Services;
using EPY.Services.TipoLogTiempoService.Client;
using EPY.Services.UserWorkQuota.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace EPY.Services.LogTiempo.Projections
{
    [Route("/v1/events/timelog")]
    [Authorize]
    public class TimeLogProjectionController : Controller
    {
        private const string DateTimeFormatPrecise = "yyyy-MM-ddTHH:mm:ssZ";

        readonly ITimeLogRepository repository;
        readonly ITimeSpanInfoService infoSvc;
        readonly IOptions<ConfigurationOptions> configurationOpt;
        readonly IReasonsInfoService reasonsSvc;
        readonly ITipoLogTiempoClient TipoLogTiempoClient;

        public TimeLogProjectionController(ITimeLogRepository repository, ITimeSpanInfoService infoSvc, IReasonsInfoService reasonsSvc, IOptions<ConfigurationOptions> optionsAccessor, ITipoLogTiempoClient TipoLogTiempoClient)
        {
            this.repository = repository;
            this.infoSvc = infoSvc;
            configurationOpt = optionsAccessor;
            this.reasonsSvc = reasonsSvc;
            this.TipoLogTiempoClient = TipoLogTiempoClient;
        }

        [HttpPut]
        public async Task CreateTimelog([FromBody] TimeLogCreatedEvent evnt)
        {
            var ttRequest = await TipoLogTiempoClient.GetTipoLogTiempoById(evnt.TipoLogTiempo);
            var TipoLogTiempo = new Models.TipoLogTiempo { Color = ttRequest.Color, Factor = ttRequest.Factor, Id = ttRequest.Id, Name = ttRequest.Name };

            // Check the factor to verify if it's fixed or not.
            if (TipoLogTiempo.Factor == 0 && (string.IsNullOrEmpty(TipoLogTiempo.Name) || TipoLogTiempo.Name.ToLower().Contains("work")) && IsDateToday(evnt.Time))
            {
                // Fetch an open timespan should one exist!
                var timeSpan = await infoSvc.GetLastTimeSpanForUser(evnt.UserId);
                var isNew = timeSpan == null;

                // Set the start or endtime of the timespan
                timeSpan = infoSvc.AddLogToTimeSpan(timeSpan, new TimeLog
                {
                    Time = DateTimeOffset.Parse(evnt.Time)
                });

                timeSpan.Type = TipoLogTiempo.Id.ToString().ToLower();

                if (isNew)
                {
                    // Create a new Timespan if there was none before
                    var newTimeSpan = new TimeLogSpan
                    {
                        Id = evnt.Id,
                        TimeStart = timeSpan.StartTime.ToString(DateTimeFormatPrecise),
                        UserId = evnt.UserId,
                        TipoLogTiempo = timeSpan.Type,
                    };

                    await repository.CreateTimeLog(newTimeSpan);
                }
                else
                {
                    // update the existing timelog
                    var updateTimeStamp = new TimeLogSpan
                    {
                        Id = timeSpan.Id,
                        TimeStart = timeSpan.StartTime.ToString(DateTimeFormatPrecise),
                        UserId = evnt.UserId,
                        TipoLogTiempo = timeSpan.Type,
                        TimeEnd = timeSpan.EndTime.Value.ToString(DateTimeFormatPrecise),
                    };

                    await repository.UpdateTimelog(updateTimeStamp);
                }
            }
            else
            {
                var fixedTimeSpan = await infoSvc.CalculateEndTimeWithType(evnt.Id, DateTimeOffset.Parse(evnt.Time), evnt.TipoLogTiempo, evnt.UserId);

                // Create a timelog with it's full data.
                await repository.CreateTimeLog(new TimeLogSpan
                {
                    Id = fixedTimeSpan.Id.ToString(),
                    TimeStart = fixedTimeSpan.StartTime.ToString(DateTimeFormatPrecise),
                    UserId = evnt.UserId,
                    TimeEnd = fixedTimeSpan.EndTime?.ToString(DateTimeFormatPrecise),
                    Edited = false,
                    TipoLogTiempo = TipoLogTiempo.Id.ToString().ToLower(),
                });
            }
        }

        [HttpPost]
        public async Task UpdateTimeLog([FromBody] TimelogUpdatedEvent evnt)
        {
            await repository.UpdateTimelog(new TimeLogSpan
            {
                Id = evnt.Id,
                TimeStart = evnt.Time,
                UserId = evnt.UserId,
                Edited = true,
                TimeEndReason = evnt.TimeEndReason,
                TimeStartReason = evnt.TimeStartReason,
                TimeEnd = evnt.TimeEnd,
            });
            if (!string.IsNullOrEmpty(evnt.TimeEndReason))
            {
                await reasonsSvc.AddReasonForUser(evnt.TimeEndReason, evnt.UserId);
            }

            if (!string.IsNullOrEmpty(evnt.TimeStartReason))
            {
                await reasonsSvc.AddReasonForUser(evnt.TimeStartReason, evnt.UserId);
            }
        }

        [HttpDelete]
        public async Task DeleteTimeLog([FromBody] TimeLogDeletedEvent evnt)
        {
            await repository.DeleteTimeLog(evnt.Id);
        }

        private bool IsDateToday(string date)
        {
            DateTimeOffset d = DateTimeOffset.Parse(date);
            if (d.Year == DateTimeOffset.Now.Year && d.Month == DateTimeOffset.Now.Month && d.Day == DateTimeOffset.Now.Day)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
