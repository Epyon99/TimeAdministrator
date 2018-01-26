using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Docugate.Services.Common.Client.RestEase;
using EPY.Services.UserWorkQuota.Client.Models;

namespace EPY.Services.UserWorkQuota.Client
{
    public class UserWorkQuotaClient : RestEaseClientBase, IUserWorkQuotaClient
    {
        public UserWorkQuotaClient()
            : base("http://epyhost:5020/v1/types/")
        {
        }

        public UserWorkQuotaClient(string baseAddress)
            : base(baseAddress)
        {
        }

        public async Task<UserWorkQuotaResponse> GetUserWorkQuota(string userid)
        {
            try
            {
                var client = GetClient<IUserWorkQuotaApiClient>();
                return await client.GetUserWorkQuota(userid);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
