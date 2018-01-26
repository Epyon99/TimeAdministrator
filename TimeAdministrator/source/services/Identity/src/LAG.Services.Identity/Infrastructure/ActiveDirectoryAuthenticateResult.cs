using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPY.Services.Identity.Infrastructure
{
    public enum ActiveDirectoryAuthenticateResult
    {
        Success,
        InvalidCredentials,
        Error,
        ConnectionError
    }
}