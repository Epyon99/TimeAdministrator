using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EPY.Services.Identity.Infrastructure
{
    public sealed class ActiveDirectoryConnectionException : Exception
    {
        public ActiveDirectoryConnectionException(string authority) : base(string.Format($"A connection error was encountered when attemping to connect to {authority} directory."))
        {
        }
    }
}