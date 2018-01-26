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
    public class BookingsControllerTests
    {
        public BookingsControllerTests()
        {
            FakeLogDeTiempo = A.Fake<ITimeLogService>();
            IOptions<ConfigurationOptions> opt = new ConfigurationOptions() as IOptions<ConfigurationOptions>;
            Subject = new BookingsController(FakeLogDeTiempo);
        }

        public BookingsController Subject { get; set; }

        public ITimeLogService FakeLogDeTiempo { get; set; }

        [Fact]
        public async void CreateTimeLog_AtManyTimes_Successfull()
        {
            // arrange
            var now = DateTime.Now;
            var datesArray = new TimelogBookingsRequest();
            var stringDates = new string[10];
            datesArray.Bookings = new DateTimeOffset[10];

            for (int i = 0; i < 10; i++)
            {
                datesArray.Bookings[i] = now.AddDays(i);
                stringDates[i] = datesArray.Bookings[i].ToString();
            }

            datesArray.TypeId = "holiday";

            var fakesResults = new List<SvcModels.LogTimeSpan>();
            for (int i = 0; i < 10; i++)
            {
                fakesResults.Add(new SvcModels.LogTimeSpan()
                {
                    Id = Guid.NewGuid().ToString(),
                    EndTime = datesArray.Bookings[i],
                    StartTime = datesArray.Bookings[i],
                });
            }

            var fakeId = Guid.NewGuid();

            A.CallTo(() => FakeLogDeTiempo.CreateMultipleBookings(A<TimelogBookingsRequest>.That.Not.IsNull()))
                .ReturnsLazily((TimelogBookingsRequest req) =>
                new ServiceResult<IEnumerable<SvcModels.LogTimeSpan>>
                {
                    Status = Results.Success,
                    Result = fakesResults as IEnumerable<SvcModels.LogTimeSpan>,
                });

            Subject
                .WithFakeContext()
                .WithFakePrincipal("popey")
                .WithClaim(x => x.Add(new Claim("sub", "popeys_userid")));

            // act
            var result = await Subject.CreateBookings(new BookingRequest()
            {
                Dates = stringDates, Typeid = "vacation",
            });

            // assert
            result.Should().BeOfType<OkObjectResult>()
                .And
                .Subject.As<OkObjectResult>()
                .Value.Should().BeOfType<List<TimeLogResponse>>();

            var response = (result as OkObjectResult).Value as List<TimeLogResponse>;

            response.Count.Should().Be(fakesResults.Count);
            foreach (var timelog in response)
            {
                timelog.Id.Should().BeNullOrEmpty();
                timelog.StartTime.Should().BeNullOrEmpty();
                timelog.EndTime.Should().BeNullOrEmpty();
                timelog.Type.Should().BeNullOrEmpty();
            }
        }

        [Fact]
        public async void CreateTimeLog_AtManyTimes_WithMalformatted_DateTimeOffsetString()
        {
            // Similar to CreateTimeLog_AtGivenTime_Successfull()
            // but malformat the datetime parameter so it cannot be parsed correctly
            // REMARKS: check the if the response of the controller matches the specs in the swagger file!!
            // arrange
            var now = DateTime.Now;
            var datesArray = new TimelogBookingsRequest();
            var stringDates = new string[10];
            datesArray.Bookings = new DateTimeOffset[10];

            for (int i = 0; i < 10; i++)
            {
                datesArray.Bookings[i] = now.AddDays(i);
                stringDates[i] = datesArray.Bookings[i].ToString();
            }

            datesArray.TypeId = "holiday";

            var fakesResults = new List<SvcModels.LogTimeSpan>();
            for (int i = 0; i < 10; i++)
            {
                fakesResults.Add(new SvcModels.LogTimeSpan()
                {
                    Id = Guid.NewGuid().ToString(),
                    EndTime = datesArray.Bookings[i],
                    StartTime = datesArray.Bookings[i],
                });
            }

            var fakeId = Guid.NewGuid();

            A.CallTo(() => FakeLogDeTiempo.CreateMultipleBookings(A<TimelogBookingsRequest>.That.Not.IsNull()))
                .ReturnsLazily((TimelogBookingsRequest req) =>
                new ServiceResult<IEnumerable<SvcModels.LogTimeSpan>>
                {
                    Status = Results.Success,
                    Result = fakesResults as IEnumerable<SvcModels.LogTimeSpan>,
                });

            Subject
                .WithFakeContext()
                .WithFakePrincipal("popey")
                .WithClaim(x => x.Add(new Claim("sub", "popeys_userid")));

            // act
            // act
            var result = await Subject.CreateBookings(new BookingRequest()
            {
                Dates = stringDates,
                Typeid = "vacation",
            });

            // assert
            result.Should().BeOfType<OkObjectResult>()
                .And
                .Subject.As<OkObjectResult>()
                .Value.Should().BeOfType<List<TimeLogResponse>>();

            var response = (result as OkObjectResult).Value as List<TimeLogResponse>;

            response.Count.Should().Be(fakesResults.Count);
            foreach (var timelog in response)
            {
                timelog.Id.Should().BeNullOrEmpty();
                timelog.StartTime.Should().BeNullOrEmpty();
                timelog.EndTime.Should().BeNullOrEmpty();
                timelog.Type.Should().BeNullOrEmpty();
            }
        }

        [Fact]
        public async void CreateTimeLog_AtManyTims_WithMissing_DateTimeOffsetString()
        {
            // Similar to CreateTimeLog_AtGivenTime_Successfull()
            // but the datetime parameter is missing (because someone did not read the specs)
            // REMARKS: check the if the response of the controller matches the specs in the swagger file!!
            // arrange
            var now = DateTime.Now;
            var datesArray = new TimelogBookingsRequest();
            var stringDates = new string[10];
            datesArray.Bookings = new DateTimeOffset[10];

            for (int i = 0; i < 10; i++)
            {
                datesArray.Bookings[i] = now.AddDays(i);
                stringDates[i] = datesArray.Bookings[i].ToString();
            }

            datesArray.TypeId = "holyday";

            var fakesResults = new List<SvcModels.LogTimeSpan>();
            for (int i = 0; i < 10; i++)
            {
                fakesResults.Add(new SvcModels.LogTimeSpan()
                {
                    Id = Guid.NewGuid().ToString(),
                    EndTime = datesArray.Bookings[i],
                    StartTime = datesArray.Bookings[i],
                });
            }

            var fakeId = Guid.NewGuid();

            A.CallTo(() => FakeLogDeTiempo.CreateMultipleBookings(A<TimelogBookingsRequest>.That.Not.IsNull()))
                .ReturnsLazily((TimelogBookingsRequest req) =>
                new ServiceResult<IEnumerable<SvcModels.LogTimeSpan>>
                {
                    Status = Results.Success,
                    Result = fakesResults as IEnumerable<SvcModels.LogTimeSpan>,
                });

            Subject
                .WithFakeContext()
                .WithFakePrincipal("popey")
                .WithClaim(x => x.Add(new Claim("sub", "popeys_userid")));

            // act
            var result = await Subject.CreateBookings(new BookingRequest()
            {
                Dates = stringDates,
                Typeid = "vacation",
            });

            // assert
            result.Should().BeOfType<OkObjectResult>()
                .And
                .Subject.As<OkObjectResult>()
                .Value.Should().BeOfType<List<TimeLogResponse>>();

            var response = (result as OkObjectResult).Value as List<TimeLogResponse>;

            response.Count.Should().Be(fakesResults.Count);
            foreach (var timelog in response)
            {
                timelog.Id.Should().BeNullOrEmpty();
                timelog.StartTime.Should().BeNullOrEmpty();
                timelog.EndTime.Should().BeNullOrEmpty();
                timelog.Type.Should().BeNullOrEmpty();
            }
        }
    }
}