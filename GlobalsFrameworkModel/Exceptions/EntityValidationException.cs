using System;

namespace GlobalsFramework.Exceptions
{
    /// <summary>
    /// The exception that is thrown when customer has declared entity that does not meet entity declaration rules.
    /// </summary>
    public class EntityValidationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:GlobalsFramework.Exceptions.EntityValidationException"/> 
        /// class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public EntityValidationException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:GlobalsFramework.Exceptions.EntityValidationException"/> 
        ///  class with a specified error message and a reference to the inner exception that is the cause of
        ///  this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        ///  (Nothing in Visual Basic) if no inner exception is specified.</param>
        public EntityValidationException(string message, Exception innerException) : 
            base(message, innerException) { }
    }
}
