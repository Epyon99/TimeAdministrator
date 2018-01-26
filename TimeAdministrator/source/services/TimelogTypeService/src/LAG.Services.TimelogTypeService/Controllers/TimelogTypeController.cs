using System;
using System.Net;
using System.Threading.Tasks;
using EPY.Services.Common.Service.Models;
using EPY.Services.TipoLogTiempoService.Controllers.Models;
using EPY.Services.TipoLogTiempoService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EPY.Services.TipoLogTiempoService.Controllers
{
    [Route("v1/types")]
    [Authorize]
    public class TipoLogTiempoController : Controller
    {
        private ITipoLogTiempoService timeLogService;

        public TipoLogTiempoController(ITipoLogTiempoService service)
        {
            timeLogService = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = await timeLogService.GetAllTipoLogTiempos();
            if (response.Status == Results.Success)
            {
                if (response.Status == Results.Success)
                {
                    var timeLogs = response.Result;
                    return Ok(timeLogs);
                }
            }

            if (response.Status == Results.BadRequest)
            {
                return BadRequest(response.Error.Message);
            }

            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTipoLogTiempoById(string id)
        {
            var response = await timeLogService.GetTipoLogTiempoById(id);
            if (response.Status == Results.Success)
            {
                return Ok(response.Result);
            }

            if (response.Status == Results.NotFound)
            {
                return NotFound(response.Error.Message);
            }

            if (response.Status == Results.Denied)
            {
                return BadRequest(response.Error.Message);
            }

            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        [HttpPut]
        public async Task<IActionResult> CreateTipoLogTiempo([FromBody]TipoLogTiempoRequest type)
        {
            var result = await timeLogService.AddTipoLogTiempo(type.Color, type.Factor, type.Name);

            if (result.Status == Results.Success)
            {
                return Ok();
            }

            if (result.Status == Results.Denied)
            {
                return BadRequest(result.Error.Message);
            }

            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTipoLogTiempo(TipoLogTiempoUpdateRequest type)
        {
            var result = await timeLogService.UpdateTipoLogTiempo(
                new Guid(type.Id), type.Color, type.Factor, type.Name);
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveTypeLogType(string id)
        {
            var result = await timeLogService.DeleteTipoLogTiempo(id);
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
    }
}
