using System.Threading.Tasks;
using EPY.Services.UserWorkQuota.Client.Models;

namespace EPY.Services.UserWorkQuota.Client
{
    public interface IUserWorkQuotaClient
    {
        Task<UserWorkQuotaResponse> GetUserWorkQuota(string userid);
    }
}