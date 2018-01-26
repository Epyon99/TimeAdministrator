using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using EPY.Services.Common.Service.Models;
using EPY.Services.LogTiempo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EPY.Services.LogTiempo.Controllers
{
    [Route("v1/search/keyword")]
    [Authorize]
    public class ReasonController : Controller
    {
        private IReasonsInfoService keywordService;

        public ReasonController(IReasonsInfoService service)
        {
            keywordService = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetKeywordsForUser([FromQuery]string keywords)
        {
            if (string.IsNullOrEmpty(keywords))
            {
                keywords = string.Empty;
            }

            var userId = ControllerContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;

            var result = await keywordService.GetReasonsForUser(keywords, userId);

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
    }
}
