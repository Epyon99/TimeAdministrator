using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EPY.Services.Common.Service.Models;
using EPY.Services.LogTiempo.Services.Models;

namespace EPY.Services.LogTiempo.Services
{
    public interface IReasonsInfoService
    {
        Task<ServiceResult<IEnumerable<string>>> GetReasonsForUser(string keywords, string user);

        Task<ServiceResult> AddReasonForUser(string keywords, string user);
    }
}
