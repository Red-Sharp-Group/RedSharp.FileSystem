using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedSharp.FileSystem.Sys.Exceptions
{
    /// <summary>
    /// Special exception that has to be thrown 
    /// in case when the input item is not supported by the service
    /// for any reason.
    /// </summary>
    public class DriveItemNotSupportedException : ArgumentException
    {
        public const String NotSupportedMessage = "Input drive item is not supported by the object.";

        public DriveItemNotSupportedException() : base(NotSupportedMessage)
        { }

        public DriveItemNotSupportedException(String paramName) : base(NotSupportedMessage, paramName)
        { }

        public DriveItemNotSupportedException(String message, String paramName) : base(message, paramName)
        { }
    }
}
