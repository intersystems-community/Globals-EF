using System;

namespace GlobalsFramework.Exceptions
{
    public class EntityValidationException : Exception
    {
        public EntityValidationException(string message):base(message){ }

        public EntityValidationException(string message, Exception innerException) : 
            base(message, innerException) { }
    }
}
