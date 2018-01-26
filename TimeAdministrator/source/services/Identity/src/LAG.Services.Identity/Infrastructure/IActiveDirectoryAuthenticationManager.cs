using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace EPY.Services.Identity.Infrastructure
{
    public interface IActiveDirectoryAuthenticationManager
    {
        /// <summary>
        /// Gets or sets wheter to use secure channel for comunnications
        /// </summary>
        bool UseSSL { get; set; }

        /// <summary>
        /// Authenticate the given credentials against an Active Directory. Should not throw exceptions at all.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<ActiveDirectoryAuthenticateResult> AuthenticateAsync(string userName, string password);

        /// <summary>
        /// Returns if a given user is active
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        bool UserIsActive(string userName);
    }
}