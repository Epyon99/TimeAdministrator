using System;

namespace EPY.Services.UserWorkQuota.Controllers.Models
{
    public class UpdateUserDailyWorkQuotaRequest
    {
        public virtual TimeSpan DailyWorkQuota { get; set; }

        public virtual string UserId { get; set; }
    }
}