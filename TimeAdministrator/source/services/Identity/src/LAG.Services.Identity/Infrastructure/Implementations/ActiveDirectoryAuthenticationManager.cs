using Microsoft.Extensions.Options;
using Novell.Directory.Ldap;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace EPY.Services.Identity.Infrastructure.Implementations
{
    public class ActiveDirectoryAuthenticationManager : IActiveDirectoryAuthenticationManager, IDisposable
    {
        private bool _connected;

        public ActiveDirectoryAuthenticationManager(string domainOrServerFqName) : this()
        {
            if (string.IsNullOrWhiteSpace(domainOrServerFqName))
            {
                throw new ArgumentException(nameof(domainOrServerFqName));
            }
            this.DomainOrServerFqName = domainOrServerFqName;
        }

        private ActiveDirectoryAuthenticationManager()
        {
            this.LdapConnection = new LdapConnection();
        }

        public bool Connected
        {
            get
            {
                return _connected;
            }
        }

        [Required]
        public string DomainOrServerFqName { get; private set; }

        /// <summary>
        /// Defaults to 389
        /// </summary>
        [DefaultValue(389)]
        public int Port { get; set; } = 389;

        public bool UseSSL
        {
            get
            {
                return this.LdapConnection.SecureSocketLayer;
            }
            set
            {
                this.LdapConnection.SecureSocketLayer = value;
            }
        }

        private LdapConnection LdapConnection { get; set; }

        public async Task<ActiveDirectoryAuthenticateResult> AuthenticateAsync(string userName, string password)
        {
            try
            {
                await Task.Run(() =>
                {
                    this.Connect();
                    LdapConnection.Bind(userName, password);
                });
                return ActiveDirectoryAuthenticateResult.Success;
            }
            catch (ActiveDirectoryConnectionException)
            {
                return ActiveDirectoryAuthenticateResult.ConnectionError;
            }
            catch (Exception e)
            {
                if (e.Message.Equals("Invalid Credentials"))
                {
                    return ActiveDirectoryAuthenticateResult.InvalidCredentials;
                }
                return ActiveDirectoryAuthenticateResult.Error;
            }
        }

        public void Connect()
        {
            if (!_connected)
            {
                try
                {
                    LdapConnection.Connect(this.DomainOrServerFqName, this.Port);
                    _connected = true;
                }
                catch (Exception)
                {
                    throw new ActiveDirectoryConnectionException(this.DomainOrServerFqName);
                }
            }
        }

        public void Disconnect()
        {
            if (_connected)
            {
                this.LdapConnection.Disconnect();
                _connected = false;
            }
        }

        public void Dispose()
        {
            this.Disconnect();
            this.LdapConnection.Dispose();
            this.LdapConnection = null;
        }

        public bool UserIsActive(string userName)
        {
            //  todo: finish this method logic. this one should return if ad user is active or not.
            return true;
        }
    }
}