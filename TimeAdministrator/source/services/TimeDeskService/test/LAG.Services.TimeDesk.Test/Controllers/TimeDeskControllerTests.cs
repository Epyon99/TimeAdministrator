using System;
using System.Collections.Generic;
using System.Security.Claims;
using FakeItEasy;
using FluentAssertions;
using EPY.Services.Common.Service.Models;
using EPY.Services.LogTiempo.Controllers;
using EPY.Services.LogTiempo.Controllers.Models;
using EPY.Services.LogTiempo.Hosting.Models;
using EPY.Services.LogTiempo.Repositories;
using EPY.Services.LogTiempo.Services.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Xunit;
using SvcModels = EPY.Services.LogTiempo.Repositories.Models;

namespace EPY.LogTiempo.Service.Test.Controllers
{
    public class LogTiempoControllerTests
    {
        public LogTiempoControllerTests()
        {
            FakeLogDeTiempo = A.Fake<ITimeLogService>();
            IOptions<ConfigurationOptions> opt = new ConfigurationOptions() as IOptions<ConfigurationOptions>;
            Subject = new LogTiempoController(FakeLogDeTiempo, opt);
        }

        public LogTiempoController Subject { get; set; }

        public ITimeLogService FakeLogDeTiempo { get; set; }

        [Fact]
        public async void CreateTimeLog_Now_Successfull()
        {
            // arrange
            var now = DateTimeOffset.Now;
            var fakeId = Guid.NewGuid();

            A.CallTo(() => FakeLogDeTiempo.CreateTimeLogNow(A<SvcModels.TimeLogRequest>.That.Not.IsNull()))
                .ReturnsLazily((SvcModels.TimeLogRequest usr) => new ServiceResult<SvcModels.LogTimeSpan>
                {
                    Status = Results.Success,
                    Result = new SvcModels.LogTimeSpan { Id = fakeId.ToString(), StartTime = now }
                });

            Subject
                .WithFakeContext()
                .WithFakePrincipal("popey")
                .WithClaim(x => x.Add(new Claim("sub", "popeys_userid")));

            // act
            var result = await Subject.CreateTimeLog(new TimeLogRequest()
            {
                DateTime = "XXX",
                LogType = "Work"
            });

            // assert
            result.Should().BeOfType<OkObjectResult>()
                .And
                .Subject.As<OkObjectResult>()
                .Value.Should().BeOfType<TimeLogResponse>();

            var response = (result as OkObjectResult).Value as TimeLogResponse;

            response.Id.Should().BeEquivalentTo(fakeId.ToString());
            response.StartTime.Should().Be(now.ToString());
        }

        [Fact]
        public void CreateTimeLog_AtGivenTime_Successfull()
        {
            // Similar to CreateTimeLog_Now_Successfull()
            // but given Subject.CreateTimeLog(new TimeLogRequest { DateTimeOffset.Now.ToString() });
        }

        [Fact]
        public void CreateTimeLog_AtGivenTime_WithMalformatted_DateTimeOffsetString()
        {
            // Similar to CreateTimeLog_AtGivenTime_Successfull()
            // but malformat the datetime parameter so it cannot be parsed correctly
            // REMARKS: check the if the response of the controller matches the specs in the swagger file!!
        }

        [Fact]
        public void CreateTimeLog_AtGivenTime_WithMissing_DateTimeOffsetString()
        {
            // Similar to CreateTimeLog_AtGivenTime_Successfull()
            // but the datetime parameter is missing (because someone did not read the specs)
            // REMARKS: check the if the response of the controller matches the specs in the swagger file!!
        }

        [Fact]
        public void GetTimeLogs_ByDayRange_ReturnsOk()
        {
            // Make sure, that everything the service return is correctly returned to the caller ()
        }

        [Fact]
        public void GetTimeLogs_ByDayRange_WithStartBeforeEndOfRange()
        {
            // Make sure, that the client will not query invalid date ranges (end before start etc.)
        }
    }
}