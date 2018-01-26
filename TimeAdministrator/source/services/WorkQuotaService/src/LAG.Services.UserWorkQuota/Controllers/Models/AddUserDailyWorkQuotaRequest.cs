using System;

namespace EPY.Services.UserWorkQuota.Controllers.Models
{
    /// <summary>
    /// Reuest object to create a new work quota
    /// </summary>
    public class AddUserDailyWorkQuotaRequest
    {
        /// <summary>
        /// Gets or sets the daily work hours, minutes and seconds
        /// </summary>
        public virtual TimeSpan DailyWorkQuota { get; set; }

        /// <summary>
        /// Gets or sets the userid of the user that gets the quota
        /// </summary>
        public virtual string UserId { get; set; }
    }
}