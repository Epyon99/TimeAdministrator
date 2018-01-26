using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestEase;

namespace EPY.Services.UserWorkQuota.Client.Models
{
    public interface IUserWorkQuotaApiClient
    {
        [Get("")]
        Task<UserWorkQuotaResponse> GetUserWorkQuota(string userid);
    }
}
