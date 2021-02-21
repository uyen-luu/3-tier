using System;

namespace NETCoreTemplate.Domain.Utilities
{
	public static class ExceptionExtensions
    {
        public static string GetErrorMessages(this Exception ex)
        {
            if (ex == null)
                return null;

            if (ex.InnerException != null)
            {
                return ex.Message + ": " + ex.InnerException.Message;
            }

            return ex.Message;
        }
    }
}
