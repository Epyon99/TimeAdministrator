using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EPY.Services.Common.Service.Models;
using EPY.Services.LogTiempo.Controllers.Models;
using EPY.Services.LogTiempo.Repositories;
using EPY.Services.LogTiempo.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EPY.Services.LogTiempo.Controllers
{
    [Route("v1/bookings")]
    [Authorize]
    public class BookingsController : Controller
    {
        public BookingsController(ITimeLogService service)
        {
            TimeLogService = service ?? throw new ArgumentNullException(nameof(service));
        }

        ITimeLogService TimeLogService { get; set; }

        [HttpPut]
        public async Task<IActionResult> CreateBookings([FromBody]BookingRequest request)
        {
            var userId = ControllerContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;

            var serviceRequest = new TimelogBookingsRequest()
            {
                Bookings = request.Dates.Select(g => new DateTimeOffset(DateTime.Parse(g))).ToArray(),
                TypeId = request.Typeid,
                UserId = userId,
            };

            var result = await TimeLogService.CreateMultipleBookings(serviceRequest);

            if (result.Status == Results.Success)
            {
                var log = result.Result.Select(g => new TimeLogResponse()
                {
                    Id = g.Id.ToString(),
                    StartTime = g.StartTime.ToString(),
                    EndTime = g.EndTime?.ToString(),
                    Type = g.Type,
                }).ToList();

                return Ok(log);
            }

            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }
}