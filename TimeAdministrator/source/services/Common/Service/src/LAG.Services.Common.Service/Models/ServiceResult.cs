using System;

namespace EPY.Services.Common.Service.Models
{
#pragma warning disable SA1402 // File may only contain a single type
    /// <summary>
    /// Generic service result
    /// </summary>
    /// <typeparam name="TResult">The of the result</typeparam>
    /// <typeparam name="TStatus">Type of the state</typeparam>
    public class ServiceResult<TResult, TStatus>
    {
        /// <summary>
        /// Gets or sets the termination state
        /// </summary>
        public TStatus Status { get; set; }

        /// <summary>
        /// Gets or sets the result object
        /// </summary>
        public TResult Result { get; set; }

        /// <summary>
        /// Gets or sets the exception if the call failed
        /// </summary>
        public Exception Error { get; set; }
    }

    /// <summary>
    /// Generic service result that uses strings as status codes
    /// </summary>
    /// <typeparam name="TResult">The of the result</typeparam>
    public class ServiceResult<TResult> : ServiceResult<TResult, string>
    {
    }

    /// <summary>
    /// Generic service result that uses strings as status codes and <see cref="object"/> as the result type
    /// </summary>
    public class ServiceResult : ServiceResult<object>
    {
    }
#pragma warning restore SA1402 // File may only contain a single type
}