using IdentityServer4.Models;
using IdentityServer4.Validation;
using System;
using System.Threading.Tasks;

namespace EPY.Services.Identity.Infrastructure.Implementations
{
    public class ActiveDirectoryOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        public ActiveDirectoryOwnerPasswordValidator(IActiveDirectoryAuthenticationManager activeDirectoryAuthenticationManager)
        {
            if (activeDirectoryAuthenticationManager == null)
            {
                throw new ArgumentNullException(nameof(activeDirectoryAuthenticationManager));
            }
            this.ActiveDirectoryAuthenticationManager = activeDirectoryAuthenticationManager;
        }

        private IActiveDirectoryAuthenticationManager ActiveDirectoryAuthenticationManager { get; set; }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var userName = context.UserName;
            var password = context.Password;
            var result = await this.ActiveDirectoryAuthenticationManager.AuthenticateAsync(userName, password);

            switch (result)
            {
                case ActiveDirectoryAuthenticateResult.Success:
                    context.Result = new GrantValidationResult(userName, "password");
                    break;

                case ActiveDirectoryAuthenticateResult.InvalidCredentials:
                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidRequest, $"Authorization for user {userName} failed.");
                    break;

                case ActiveDirectoryAuthenticateResult.Error:
                    throw new InvalidOperationException("A generic error was encountered");
                case ActiveDirectoryAuthenticateResult.ConnectionError:
                    throw new InvalidOperationException("Connection error");
                default:
                    throw new InvalidOperationException($"Unkown error: {result.ToString()}");
            }
        }
    }
}