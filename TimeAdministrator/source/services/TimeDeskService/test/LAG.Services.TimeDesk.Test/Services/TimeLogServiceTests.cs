using System;
using System.EventSourcing.Client;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using EPY.Services.Common.Service.Models;
using EPY.Services.LogTiempo.Repositories;
using EPY.Services.TipoLogTiempoService.Client;
using EPY.Services.UserWorkQuota.Client;
using Xunit;
using SvcModels = EPY.Services.LogTiempo.Repositories.Models;

namespace EPY.LogTiempo.Service.Test.Services
{
    public class TimeLogServiceTests
    {
        public TimeLogServiceTests()
        {
            TimeLogRepository = A.Fake<ITimeLogRepository>();
            EventClient = A.Fake<IEventClient>();

            SpanInfoService = A.Fake<ITimeSpanInfoService>();
            A.CallTo(() => SpanInfoService.GetLastTimeSpanForUser(A<string>._)).Returns(Task.FromResult<SvcModels.LogTimeSpan>(null));
            A.CallTo(() => SpanInfoService.AddLogToTimeSpan(A<SvcModels.LogTimeSpan>._, A<SvcModels.TimeLog>._))
                .ReturnsLazily<SvcModels.LogTimeSpan, SvcModels.LogTimeSpan, SvcModels.TimeLog>(
                (span, timelog) =>
                {
                    return new SvcModels.LogTimeSpan
                    {
                        Id = timelog.Id.ToString(),
                        StartTime = timelog.Time
                    };
                });
        }

        public ITimeLogRepository TimeLogRepository { get; private set; }

        public IEventClient EventClient { get; private set; }

        public ITimeSpanInfoService SpanInfoService { get; private set; }

        [Fact]
        public async void Create_TimeLog_Simple()
        {
            // arrange
            var service = new TimeLogService(TimeLogRepository, EventClient, SpanInfoService);

            // act
            var result = await service.CreateTimeLogNow(new SvcModels.TimeLogRequest { UserId = "fakeUser" });

            // assert
            result.Should().NotBeNull();
            result.Status.Should().Be(Results.Success);

            result.Result.Id.Should().NotBeNullOrWhiteSpace();
            result.Result.StartTime.Should().NotBe(default(DateTimeOffset));

            A.CallTo(() => EventClient.Publish(A<SvcModels.Events.TimeLogCreatedEvent>.That.Matches(
                x => x.Time == result.Result.StartTime.ToString(TimeLogService.DateTimeFormatPrecise)
                     && x.Id == result.Result.Id.ToString().ToLower()
                     && x.TipoLogTiempo == "default")))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void Create_TimeLog_WithType()
        {
            // arrange
            var service = new TimeLogService(TimeLogRepository, EventClient, SpanInfoService);

            // act
            var result = await service.CreateTimeLogNow(new SvcModels.TimeLogRequest { UserId = "fakeUser", LogType = "work" });

            // assert
            result.Should().NotBeNull();
            result.Status.Should().Be(Results.Success);

            result.Result.Id.Should().NotBeNullOrWhiteSpace();
            result.Result.StartTime.Should().NotBe(default(DateTimeOffset));

            A.CallTo(() => EventClient.Publish(A<SvcModels.Events.TimeLogCreatedEvent>.That.Matches(
                x => x.Time == result.Result.StartTime.ToString(TimeLogService.DateTimeFormatPrecise)
                     && x.Id == result.Result.Id.ToString().ToLower()
                     && x.TipoLogTiempo == "work")))
                .MustHaveHappened(Repeated.Exactly.Once);
        }

        [Fact]
        public async void Create_TimeLog_IfSpanExists()
        {
            // arrange
            var existingSpanId = Guid.NewGuid().ToString();
            A.CallTo(() => SpanInfoService.GetLastTimeSpanForUser(A<string>._))
                .Returns(Task.FromResult(
                    new SvcModels.LogTimeSpan
                    {
                        Id = existingSpanId
                    }));
            A.CallTo(() => SpanInfoService.AddLogToTimeSpan(A<SvcModels.LogTimeSpan>._, A<SvcModels.TimeLog>._))
                .ReturnsLazily<SvcModels.LogTimeSpan, SvcModels.LogTimeSpan, SvcModels.TimeLog>(
                (span, timelog) =>
                {
                    span.EndTime = timelog.Time;
                    return span;
                });

            var service = new TimeLogService(TimeLogRepository, EventClient, SpanInfoService);

            // act
            var result = await service.CreateTimeLogNow(new SvcModels.TimeLogRequest { UserId = "fakeUser" });

            // assert
            result.Should().NotBeNull();
            result.Status.Should().Be(Results.Success);

            result.Result.Id.Should().NotBeNullOrWhiteSpace().And.BeEquivalentTo(existingSpanId);
            result.Result.EndTime.Should().NotBe(default(DateTimeOffset));

            A.CallTo(() => EventClient.Publish(A<SvcModels.Events.TimeLogCreatedEvent>.That.Matches(
                x => x.Time == result.Result.EndTime.Value.ToString(TimeLogService.DateTimeFormatPrecise)
                     && x.TipoLogTiempo == "default")))
                .MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}