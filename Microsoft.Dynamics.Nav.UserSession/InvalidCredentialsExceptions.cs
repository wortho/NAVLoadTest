using System;

namespace Microsoft.Dynamics.Nav.UserSession
{
    /// <summary>
    /// Test exception class which signals the invalid credentials fault to the test side.
    /// </summary>
    [SerializableAttribute] 
    public class InvalidCredentialsException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCredentialsException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public InvalidCredentialsException(string message)
            : base(message)
        {
        }
    }
}
