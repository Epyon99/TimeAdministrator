using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EPY.Services.Common.Service.Models;
using EPY.Services.LogTiempo.Controllers.Models;
using EPY.Services.LogTiempo.Hosting.Models;
using EPY.Services.LogTiempo.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Svc = EPY.Services.LogTiempo.Repositories.Models;

namespace EPY.Services.LogTiempo.Controllers
{
    [Route("v1/logs")]
    [Authorize]
    public class LogTiempoController : Controller
    {
        public LogTiempoController(ITimeLogService service, IOptions<ConfigurationOptions> configurationOpt)
        {
            TimeLogService = service ?? throw new ArgumentNullException(nameof(service));
            ConfigurationOpt = configurationOpt;
        }

        ITimeLogService TimeLogService { get; set; }

        IOptions<ConfigurationOptions> ConfigurationOpt { get; set; }

        [HttpPut]
        public async Task<ActionResult> CreateTimeLog([FromBody] TimeLogRequest request)
        {
            var userId = ControllerContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;

            var serviceRequest = new Svc.TimeLogRequest()
            {
                UserId = userId,
                LogType = request.LogType,
            };
            if (!string.IsNullOrEmpty(request.DateTime) || request.DateTime != null)
            {
                serviceRequest.Date = DateTime.Parse(request.DateTime);
            }

            var result = (string.IsNullOrEmpty(request.DateTime) || request.DateTime == null) ? await TimeLogService.CreateTimeLogNow(serviceRequest) : await TimeLogService.CreateTimeLogAtDate(serviceRequest);

            if (result.Status == Results.Success)
            {
                var log = result.Result;

                return Ok(new TimeLogResponse
                {
                    Id = log.Id.ToLower(),
                    StartTime = log.StartTime.ToString(),
                    EndTime = log.EndTime?.ToString(),
                    Type = log.Type,
                });
            }

            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        [HttpGet]
        public async Task<IActionResult> GetTimeLogsByDayRange([FromQuery]string from, [FromQuery]string till)
        {
            var userId = ControllerContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;

            DateTimeOffset fromDate;
            DateTimeOffset tillDate;

            if (!(DateTimeOffset.TryParse(from, out fromDate) && DateTimeOffset.TryParse(till, out tillDate)))
            {
                return BadRequest("The parameters from and till dates have to be in the format yyyy-MM-ddTHH:mm:ssZ.");
            }

            var result = await TimeLogService.GetTimeLogsByDaysRange(userId, fromDate, tillDate);

            if (result.Status == Results.Success)
            {
                var timeLogs = result.Result;

                return Ok(timeLogs);
            }

            if (result.Status == Results.BadRequest)
            {
                return BadRequest(result.Error.Message);
            }

            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTimeLog(string id)
        {
            var userId = ControllerContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest($"The parameter id cannot be <null> or <whitespace>");
            }

            var result = await TimeLogService.DeleteTimeLog(userId, id);

            if (result.Status == Results.Success)
            {
                return Ok();
            }

            if (result.Status == Results.NotFound)
            {
                return NotFound(result.Error.Message);
            }

            if (result.Status == Results.Denied)
            {
                return BadRequest(result.Error.Message);
            }

            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTimeLog([FromBody]TimeEditRequest req)
        {
            var userId = ControllerContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
            var result = await TimeLogService.UpdateTimeLog(
                req.Id,
                userId,
                req.TimeStart,
                req.TimeEnd,
                req.TimeEndReason,
                req.TimeStartReason);

            if (result.Status == Results.Success)
            {
                return Ok(result.Result);
            }

            if (result.Status == Results.NotFound)
            {
                return NotFound(result.Error.Message);
            }

            if (result.Status == Results.Denied)
            {
                return BadRequest(result.Error.Message);
            }

            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }
}