using IdentityServer4.Models;
using IdentityServer4.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EPY.Services.Identity.Infrastructure.Implementations
{
    public class ActiveDirectoryProfileService : IProfileService
    {
        public ActiveDirectoryProfileService(IActiveDirectoryAuthenticationManager activeDirectoryAuthenticationManager)
        {
            if (activeDirectoryAuthenticationManager == null)
            {
                throw new ArgumentNullException(nameof(activeDirectoryAuthenticationManager));
            }

            this.ActiveDirectoryAuthenticationManager = activeDirectoryAuthenticationManager;
        }

        private IActiveDirectoryAuthenticationManager ActiveDirectoryAuthenticationManager { get; set; }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            try
            {
                var userName = await Task.FromResult(GetUserName(context.Subject.Claims));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var userName = GetUserName(context.Subject.Claims);
            await Task.Run(() => context.IsActive = this.ActiveDirectoryAuthenticationManager.UserIsActive(userName: userName));
        }

        private static string GetUserName(IEnumerable<Claim> claims)
        {
            string userName = claims.ToList().Find(s => s.Type == "sub").Value;
            return userName;
        }
    }
}