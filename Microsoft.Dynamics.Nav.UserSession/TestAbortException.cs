using System;

namespace Microsoft.Dynamics.Nav.UserSession
{
    /// <summary>
    /// Used to abort a test e.g. when an unexpected dialog is shown.
    /// </summary>
    [SerializableAttribute] 
    public class TestAbortException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestAbortException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public TestAbortException(string message)
            : base(message)
        {

        }
    }
}