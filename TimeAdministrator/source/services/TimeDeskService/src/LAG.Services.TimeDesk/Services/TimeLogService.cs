using System;
using System.Collections.Generic;
using System.EventSourcing.Client;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using EnsureThat;
using EPY.Services.Common.Service.Exceptions;
using EPY.Services.Common.Service.Models;
using EPY.Services.LogTiempo.Repositories.Models;
using EPY.Services.LogTiempo.Repositories.Models.Events;
using EPY.Services.LogTiempo.Services.Models;

namespace EPY.Services.LogTiempo.Repositories
{
    public class TimeLogService : ITimeLogService
    {
        public const string DateTimeFormatPrecise = "yyyy-MM-ddTHH:mm:ssZ";
        readonly IEventClient eventClient;

        public TimeLogService(ITimeLogRepository timeLogRepository, IEventClient eventClient, ITimeSpanInfoService infoService)
        {
            this.eventClient = eventClient;
            TimeLogRepository = timeLogRepository;
            TimeSpanInfoService = infoService;
        }

        ITimeLogRepository TimeLogRepository { get; set; }

        ITimeSpanInfoService TimeSpanInfoService { get; set; }

        public async Task<ServiceResult<IEnumerable<LogTimeSpan>>> CreateMultipleBookings(TimelogBookingsRequest booking)
        {
            Ensure.That(booking.Bookings, nameof(booking.Bookings)).IsNotNull();

            Ensure.That(booking.UserId, nameof(booking.UserId))
                .WithException(x => new ServiceException($"The parameter {x.Name} cannot be <null> or <whitespace>"))
                .IsNotNullOrWhiteSpace();

            // Should the timelog type not have been set, use the timelog type default
            if (string.IsNullOrWhiteSpace(booking.TypeId))
            {
                booking.TypeId = "default";
            }

            try
            {
                var collection = new List<LogTimeSpan>();
                foreach (var timeSpan in booking.Bookings)
                {
                    var newGuid = Guid.NewGuid().ToString();
                    await eventClient.Publish(new TimeLogCreatedEvent
                    {
                        Id = newGuid,
                        Time = timeSpan.ToString(DateTimeFormatPrecise, CultureInfo.InvariantCulture),
                        UserId = booking.UserId,
                        TipoLogTiempo = booking.TypeId,
                    });
                    collection.Add(new LogTimeSpan()
                    {
                        Id = newGuid,
                        StartTime = timeSpan,
                        Type = booking.TypeId,
                    });
                }

                return new ServiceResult<IEnumerable<LogTimeSpan>>
                {
                    Status = Results.Success,
                    Result = collection,
                };
            }
            catch (Exception e)
            {
                return new ServiceResult<IEnumerable<LogTimeSpan>>
                {
                    Status = Results.Failed,
                    Error = e
                };
            }
        }

        public async Task<ServiceResult<LogTimeSpan>> CreateTimeLogNow(TimeLogRequest timelog)
        {
            Ensure.That(timelog, nameof(timelog)).IsNotNull();

            Ensure.That(timelog.UserId, nameof(timelog.UserId))
                .WithException(x => new ServiceException($"The parameter {x.Name} cannot be <null> or <whitespace>"))
                .IsNotNullOrWhiteSpace();

            // Should the timelog type not have been set, use the timelog type default
            if (string.IsNullOrWhiteSpace(timelog.LogType))
            {
                timelog.LogType = "default";
            }

            // See if there is an opent timespan!
            var currentTimeSpan = await TimeSpanInfoService.GetLastTimeSpanForUser(timelog.UserId);

            var newTimeLog = new TimeLog
            {
                Id = Guid.NewGuid(),
                Time = DateTimeOffset.UtcNow,
            };

            currentTimeSpan = TimeSpanInfoService.AddLogToTimeSpan(currentTimeSpan, newTimeLog);

            try
            {
                await eventClient.Publish(new TimeLogCreatedEvent
                {
                    Id = newTimeLog.Id.ToString().ToLower(),
                    Time = newTimeLog.Time.ToString(DateTimeFormatPrecise, CultureInfo.InvariantCulture),
                    UserId = timelog.UserId,
                    TipoLogTiempo = timelog.LogType,
                });

                return new ServiceResult<LogTimeSpan>
                {
                    Status = Results.Success,
                    Result = currentTimeSpan
                };
            }
            catch (Exception e)
            {
                return new ServiceResult<LogTimeSpan>
                {
                    Status = Results.Failed,
                    Error = e
                };
            }
        }

        public async Task<ServiceResult<LogTimeSpan>> CreateTimeLogAtDate(TimeLogRequest timelog)
        {
            Ensure.That(timelog, nameof(timelog)).IsNotNull();

            Ensure.That(timelog.UserId, nameof(timelog.UserId))
                .WithException(x => new ServiceException($"The parameter {x.Name} cannot be <null> or <whitespace>"))
                .IsNotNullOrWhiteSpace();

            // Should the timelog type not have been set, use the timelog type default
            if (string.IsNullOrWhiteSpace(timelog.LogType))
            {
                timelog.LogType = "default";
            }

            var newDate = new DateTimeOffset(timelog.Date.Year, timelog.Date.Month, timelog.Date.Day, 0, 10, 0, new TimeSpan(0));

            var currentTimeSpan = await TimeSpanInfoService.CalculateEndTimeWithType(Guid.NewGuid().ToString(), DateTimeOffset.Parse(newDate.ToString(DateTimeFormatPrecise)), timelog.LogType, timelog.UserId);

            try
            {
                await eventClient.Publish(new TimeLogCreatedEvent
                {
                    Id = currentTimeSpan.Id.ToString().ToLower(),
                    Time = currentTimeSpan.StartTime.ToString(DateTimeFormatPrecise, CultureInfo.InvariantCulture),
                    UserId = timelog.UserId,
                    TipoLogTiempo = timelog.LogType,
                });

                return new ServiceResult<LogTimeSpan>
                {
                    Status = Results.Success,
                    Result = currentTimeSpan
                };
            }
            catch (Exception e)
            {
                return new ServiceResult<LogTimeSpan>
                {
                    Status = Results.Failed,
                    Error = e
                };
            }
        }

        public async Task<ServiceResult> DeleteTimeLog(string userid, string timespanid)
        {
            try
            {
                var logTimeSpan = await TimeLogRepository.ReadTimeLogById(timespanid);

                if (logTimeSpan == null)
                {
                    return new ServiceResult
                    {
                        Status = Results.NotFound,
                        Error = new Exception("The object was not found"),
                    };
                }

                if (logTimeSpan.UserId != userid)
                {
                    return new ServiceResult
                    {
                        Status = Results.Denied,
                        Error = new Exception("The user is not authorized"),
                    };
                }

                await eventClient.Publish(new TimeSpanDeletedEvent
                {
                    Id = timespanid
                });

                return new ServiceResult
                {
                    Status = Results.Success
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

        public async Task<ServiceResult<IEnumerable<LogTimeSpan>>> GetTimeLogsByDaysRange(string userid, DateTimeOffset from, DateTimeOffset? till)
        {
            Ensure.That(userid, nameof(userid))
                .WithException(x => new ServiceException($"The parameter {x.Name} cannot be <null> or <whitespace>"))
                .IsNotNullOrWhiteSpace();

            Ensure.That(from, nameof(from))
                .WithException(x => new ServiceException($"The from date {x.Name} cannot be later than till date"))
                .IsLte(till.Value);

            if (!till.HasValue)
            {
                till = new DateTimeOffset(DateTime.Now);
            }

            try
            {
                var timelogs = await TimeLogRepository.ReadLogsByDateRange(userid, from.Date, till.Value.DateTime);
                var offsetTimelogs = timelogs.Select(g => new LogTimeSpan
                {
                    Id = g.Id,
                    StartTime = DateTimeOffset.Parse(g.TimeStart),
                    EndTime = g.TimeEnd != null ? DateTimeOffset.Parse(g.TimeEnd) as DateTimeOffset? : null,
                    StartReason = g.TimeStartReason,
                    EndReason = g.TimeEndReason,
                    Type = g.TipoLogTiempo,
                    Edited = g.Edited,
                });

                return new ServiceResult<IEnumerable<LogTimeSpan>>
                {
                    Result = offsetTimelogs,
                    Status = Results.Success,
                };
            }
            catch (Exception e)
            {
                return new ServiceResult<IEnumerable<LogTimeSpan>>
                {
                    Error = e,
                    Status = Results.Failed,
                };
            }
        }

        public async Task<ServiceResult<TimeLogUpdate>> UpdateTimeLog(string logId, string userId, string timeStart, string timeEnd, string timeEndReason, string timeStartReason)
        {
            Ensure.That(timeStart, nameof(timeStart))
                .WithException(x => new ServiceException($"The parameter {x.Name} cannot be <null> or <whitespace>"))
                .IsNotNullOrWhiteSpace();
            Ensure.That(logId, nameof(logId))
                .WithException(x => new ServiceException($"The parameter {x.Name} cannot be <null> or <whitespace>"))
                .IsNotNullOrWhiteSpace();
            Ensure.That(userId, nameof(userId))
                .WithException(x => new ServiceException($"The parameter {x.Name} cannot be <null> or <whitespace>"))
                .IsNotNullOrWhiteSpace();

            try
            {
                var logTimeSpan = await TimeLogRepository.ReadTimeLogById(logId);

                if (logTimeSpan == null)
                {
                    return new ServiceResult<TimeLogUpdate>
                    {
                        Status = Results.NotFound,
                        Error = new Exception("The object was not found"),
                    };
                }

                if (logTimeSpan.UserId != userId)
                {
                    return new ServiceResult<TimeLogUpdate>
                    {
                        Status = Results.Denied,
                        Error = new Exception("The user is not authorized"),
                    };
                }

                var updateEvent = new TimeSpanUpdatedEvent
                {
                    Id = logId,
                    TimeEnd = timeEnd,
                    TimeEndReason = timeEndReason,
                    Time = timeStart,
                    TimeStartReason = timeStartReason,
                    UserId = userId,
                };
                if (updateEvent.TimeEnd == null)
                {
                    updateEvent.TimeEnd = logTimeSpan.TimeEnd;
                }

                await eventClient.Publish(updateEvent);

                return new ServiceResult<TimeLogUpdate>
                {
                    Status = Results.Success,
                    Result = new TimeLogUpdate
                    {
                        Id = logId,
                        UserId = userId,
                        TimeStart = timeStart,
                        TimeEnd = timeEnd,
                        TimeEndReason = timeEndReason,
                        TimeStartReason = timeStartReason,
                    }
                };
            }
            catch (Exception e)
            {
                return new ServiceResult<TimeLogUpdate>
                {
                    Status = Results.Failed,
                    Error = e
                };
            }
        }
    }
}