using System;
using System.Collections.Generic;
using System.Net;
using FluentAssertions;
using EPY.Services.Common.Service.Exceptions;
using EPY.Services.UserWorkQuota.Controllers;
using EPY.Services.UserWorkQuota.Controllers.Models;
using EPY.Services.UserWorkQuota.Repositories.Memory;
using EPY.Services.UserWorkQuota.Repositories.Models;
using EPY.Services.UserWorkQuota.Services;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace EPY.Services.UserWorkQuota.Test.Controllers
{
    public class UserWorkQuotaControllerTest
    {
        public UserWorkQuotaControllerTest()
        {
            MemoryWorkQuotaRepository repo = new MemoryWorkQuotaRepository();
            repo.WorkQuotasById = new Dictionary<string, WorkQuota>
            {
                { "A", new WorkQuota { Id = "A1", DailyWorkQuota = "23:00:00", UserId = "A" } },
                { "B", new WorkQuota { Id = "B2", DailyWorkQuota = "23:00:00", UserId = "B" } },
                { "C", new WorkQuota { Id = "C3", DailyWorkQuota = "23:00:00", UserId = "C" } },
                { "D", new WorkQuota { Id = "D4", DailyWorkQuota = "23:00:00", UserId = "D" } },
                { "E", new WorkQuota { Id = "E5", DailyWorkQuota = "23:00:00", UserId = "E" } },
                { "F", new WorkQuota { Id = "F6", DailyWorkQuota = "23:00:00", UserId = "F" } },
                { "G", new WorkQuota { Id = "G7", DailyWorkQuota = "23:00:00", UserId = "G" } },
                { "H", new WorkQuota { Id = "H8", DailyWorkQuota = "23:00:00", UserId = "H" } },
            };

            FakeQuotaService = new UserCuotaDeTrabajo(repo);
            Subject = new WorkQuotaController(FakeQuotaService);
            FailureQuotaService = new UserCuotaDeTrabajo(null);
            FailureSubject = new WorkQuotaController(FailureQuotaService);
        }

        public WorkQuotaController Subject { get; set; }

        public IUserCuotaDeTrabajo FakeQuotaService { get; set; }

        public IUserCuotaDeTrabajo FailureQuotaService { get; set; }

        public WorkQuotaController FailureSubject { get; set; }

        [Fact]
        public async void CreateUserWorkQuota_Successfully()
        {
            // arrange
            var workQuota = DateTime.Now.TimeOfDay;
            var fakeId = Guid.NewGuid();

            // act
            var result = await Subject.AddUserDailyWorkQuota(new AddUserDailyWorkQuotaRequest { UserId = fakeId.ToString(), DailyWorkQuota = workQuota });

            // assert
            result.Should().BeOfType<OkObjectResult>();

            var response = (result as OkObjectResult).Value as WorkQuota;

            response.UserId.Should().BeEquivalentTo(fakeId.ToString());
            response.DailyWorkQuota.Should().Be(workQuota.ToString());
        }

        [Fact]
        public async void CreateUserWorkQuota_WrongParameters()
        {
            // arrange
            var workQuota = DateTime.Now.TimeOfDay;
            var fakeId = Guid.NewGuid();
            var wrongId = string.Empty;

            // act
            var responseWrongId = await Subject.AddUserDailyWorkQuota(new AddUserDailyWorkQuotaRequest { UserId = wrongId, DailyWorkQuota = workQuota });

            // assert
            responseWrongId.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async void CreateUserWorkQuota_ServiceFailure()
        {
            // arrange
            var workQuota = DateTime.Now.TimeOfDay;
            var fakeId = Guid.NewGuid();

            // act
            var result = await FailureSubject.AddUserDailyWorkQuota(new AddUserDailyWorkQuotaRequest { UserId = fakeId.ToString(), DailyWorkQuota = workQuota });

            // assert
            var response = result as StatusCodeResult;
            response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async void ReadUserWorkQuota_Successfully()
        {
            // arrange
            var testUser = "A";
            var testUserWorkQuota = "23:00:00";

            // act
            var result = await Subject.GetUserDailyWorkQuota(testUser);

            // assert
            result.Should().BeOfType<OkObjectResult>();

            var response = (result as OkObjectResult).Value as GetUserDailyWorkQuotaResponse;

            response.UserDailyWorkQuota.Should().BeSameAs(testUserWorkQuota);
        }

        [Fact]
        public async void ReadUserWorkQuota_WrongParameters()
        {
            // arrange
            var testUser = string.Empty;

            // act
            var result = await Subject.GetUserDailyWorkQuota(testUser);

            // assert
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async void ReadUserWorkQuota_ServiceFailure()
        {
            // arrange
            var testUser = "A";

            // act
            var result = await FailureSubject.GetUserDailyWorkQuota(testUser);

            // assert
            var response = result as StatusCodeResult;
            response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async void UpdateUserWorkQuota_Successfully()
        {
            // arrange
            var workQuota = DateTime.Now.TimeOfDay;
            var testUser = "B";

            // act
            var result = await Subject.UpdateUserDailyWorkQuota(new UpdateUserDailyWorkQuotaRequest { UserId = testUser, DailyWorkQuota = workQuota });

            // assert
            result.Should().BeOfType<OkObjectResult>();

            var response = (result as OkObjectResult).Value as WorkQuota;

            response.UserId.Should().BeEquivalentTo(testUser);
            response.DailyWorkQuota.Should().Be(workQuota.ToString());
        }

        [Fact]
        public async void UpdateUserWorkQuota_WrongParameters()
        {
            // arrange
            var workQuota = DateTime.Now.TimeOfDay;
            var wrongUser = string.Empty;

            // act
            var result = await Subject.UpdateUserDailyWorkQuota(new UpdateUserDailyWorkQuotaRequest { UserId = wrongUser, DailyWorkQuota = workQuota });

            // assert
            result.Should().BeOfType<BadRequestResult>();
        }

        [Fact]
        public async void UpdateUserWorkQuota_ServiceFailure()
        {
            // arrange
            var workQuota = DateTime.Now.TimeOfDay;
            var testUser = "D";

            // act
            var result = await FailureSubject.UpdateUserDailyWorkQuota(new UpdateUserDailyWorkQuotaRequest { UserId = testUser, DailyWorkQuota = workQuota });

            // assert
            var response = result as StatusCodeResult;
            response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }

        [Fact]
        public async void DeleteUserWorkQuota_Successfully()
        {
            // arrange
            var testUser = "E";

            // act
            var result = await Subject.DeleteUserDailyWorkQuota(testUser);

            // assert
            result.Should().BeOfType<OkResult>();
        }

        [Fact]
        public async void DeleteUserWorkQuota_WrongParameters()
        {
            // arrange
            var testUser = string.Empty;

            ServiceException wrongUserResult = null;

            // act
            try
            {
                await Subject.DeleteUserDailyWorkQuota(testUser);
            }
            catch (ServiceException e)
            {
                wrongUserResult = e;
            }

            // assert
            var wrongUserResponse = (wrongUserResult as ServiceException).Message;
            wrongUserResponse.Should().Contain("userid");
        }

        [Fact]
        public async void DeleteUserWorkQuota_ServiceFailure()
        {
            // arrange
            var testUser = "A";

            // act
            var result = await FailureSubject.DeleteUserDailyWorkQuota(testUser);

            // assert
            var response = result as StatusCodeResult;
            response.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
        }
    }
}
