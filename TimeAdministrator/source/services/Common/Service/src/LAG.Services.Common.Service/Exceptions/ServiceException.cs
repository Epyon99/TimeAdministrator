using System;

namespace EPY.Services.Common.Service.Exceptions
{
    /// <summary>
    /// A <see cref="ServiceException "/> should only be thrown in the service layer and indicates that some kind of business functionality has gone wrong
    /// </summary>
    public class ServiceException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceException"/> class.
        /// </summary>
        /// <param name="message">the error message</param>
        public ServiceException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceException"/> class.
        /// </summary>
        /// <param name="message">the error message</param>
        /// <param name="e">the underlaying <see cref="Exception"/></param>
        public ServiceException(string message, Exception e)
            : base(message, e)
        {
        }
    }
}