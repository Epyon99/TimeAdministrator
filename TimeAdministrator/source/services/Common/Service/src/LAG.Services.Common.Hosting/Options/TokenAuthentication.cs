namespace EPY.Services.Common.Hosting.Options
{
    /// <summary>
    /// Authentication object read from the configuration
    /// </summary>
    public class TokenAuthentication
    {
        /// <summary>
        /// Gets or sets the token validation authority
        /// </summary>
        public string Authority { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use https when validating the token or not
        /// </summary>
        public bool RequireHttpsMetadata { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to cache the tokens or not
        /// </summary>
        public bool EnableCaching { get; set; }

        /// <summary>
        /// Gets or sets the desired scope
        /// </summary>
        public string ScopeName { get; set; }

        /// <summary>
        /// Gets or sets the scopes secret
        /// </summary>
        public string ScopeSecret { get; set; }
    }
}
