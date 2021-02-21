using System;

namespace NETCoreTemplate.Domain.Utilities
{
    public class AppException : Exception
    {
        public AppException(string message) : base(message) { }

        public AppException(string message, Exception innerException) : base(message, innerException) { }
    }
}
