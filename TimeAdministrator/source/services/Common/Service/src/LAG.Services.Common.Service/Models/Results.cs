namespace EPY.Services.Common.Service.Models
{
    /// <summary>
    /// Provides a number of possible service results
    /// </summary>
    public class Results
    {
        /// <summary>
        /// The desired operation was successfull
        /// </summary>
        public const string Success = "Success";

        /// <summary>
        /// A desired resource was not found
        /// </summary>
        public const string NotFound = "NotFound";

        /// <summary>
        /// The requested operation was denied (does not necessarly imply insufficent user permissions)
        /// </summary>
        public const string Denied = "Denied";

        /// <summary>
        /// The requested operation failed for an unknown reason (avoid using this status code)
        /// </summary>
        public const string Failed = "Failed";

        /// <summary>
        /// The request have invalid parameters.
        /// </summary>
        public const string BadRequest = "BadRequest";
    }
}