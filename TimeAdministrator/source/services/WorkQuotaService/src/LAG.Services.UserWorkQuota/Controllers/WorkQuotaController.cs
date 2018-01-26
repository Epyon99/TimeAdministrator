using System;
using System.Net;
using System.Threading.Tasks;
using EPY.Services.Common.Service.Models;
using EPY.Services.UserWorkQuota.Controllers.Models;
using EPY.Services.UserWorkQuota.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EPY.Services.UserWorkQuota.Controllers
{
    /// <summary>
    /// Main controller for user work quota managment
    /// </summary>
    [Route("v1/daily")]
    [Authorize]
    public class WorkQuotaController : Controller
    {
        readonly IUserCuotaDeTrabajo service;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkQuotaController"/> class.
        /// </summary>
        /// <param name="service">the underlaying <see cref="IUserCuotaDeTrabajo"/></param>
        public WorkQuotaController(IUserCuotaDeTrabajo service)
        {
            if (this.service == null)
            {
                this.service = service;
            }
        }

        /// <summary>
        /// Adds a daily work quota for a given user.
        /// </summary>
        /// <param name="request">The request object.</param>
        /// <returns>A async <see cref="Task"/></returns>
        [HttpPut]
        public async Task<IActionResult> AddUserDailyWorkQuota([FromBody]AddUserDailyWorkQuotaRequest request)
        {
            if (string.IsNullOrEmpty(request.UserId) || request.DailyWorkQuota == default(TimeSpan))
            {
                return BadRequest();
            }

            var result = await service.AddUserWorkQuota(request.UserId, request.DailyWorkQuota.ToString());
            if (result.Status == Results.Success)
            {
                var userWorkQuota = result.Result;
                return Ok(userWorkQuota);
            }

            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        /// <summary>
        /// Deletes a given user daily work quota.
        /// </summary>
        /// <param name="userid">The user id</param>
        /// <returns>A async <see cref="Task"/></returns>
        [HttpDelete("{userid}")]
        public async Task<IActionResult> DeleteUserDailyWorkQuota([FromRoute]string userid)
        {
            if (string.IsNullOrEmpty(userid))
            {
                return BadRequest($"The parameter userid cannot be <null> or <whitespace>");
            }

            var result = await service.DeleteUserWorkQuota(userid);

            if (result.Status == Results.Success)
            {
                return Ok();
            }

            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        /// <summary>
        /// Returns a user daily work quota
        /// </summary>
        /// <param name="userid">The user id</param>
        /// <returns>the requested user daily time quota response</returns>
        [HttpGet("{userid}")]
        public async Task<IActionResult> GetUserDailyWorkQuota([FromRoute]string userid)
        {
            if (string.IsNullOrEmpty(userid))
            {
                return BadRequest($"The parameter userid cannot be <null> or <whitespace>");
            }

            var result = await service.GetUserWorkQuotaByUserId(userid);

            if (result.Status == Results.Success)
            {
                return Ok(new GetUserDailyWorkQuotaResponse
                {
                    UserDailyWorkQuota = result.Result.DailyWorkQuota
                });
            }

            if (result.Status == Results.NotFound)
            {
                return NotFound();
            }

            return StatusCode((int)HttpStatusCode.InternalServerError);
        }

        /// <summary>
        /// Updates a given user daily work quota.
        /// </summary>
        /// <param name="request">The operation request object.</param>
        /// <returns>A async <see cref="Task"/></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateUserDailyWorkQuota([FromBody]UpdateUserDailyWorkQuotaRequest request)
        {
            if (string.IsNullOrEmpty(request.UserId) || false)
            {
                return BadRequest();
            }

            var result = await service.UpdateUserWorkQuota(request.UserId, request.DailyWorkQuota.ToString());

            if (result.Status == Results.Success)
            {
                return Ok(result.Result);
            }

            if (result.Status == Results.NotFound)
            {
                return NotFound();
            }

            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }
}