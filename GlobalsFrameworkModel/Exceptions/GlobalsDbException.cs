using System;

namespace GlobalsFramework.Exceptions
{
    public class GlobalsDbException : Exception
    {
        public GlobalsDbException(string message) : base(message) { }

        public GlobalsDbException(string message, Exception innerException) :
            base(message, innerException) { }
    }
}
