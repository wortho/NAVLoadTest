using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Dynamics.Nav.TestUtilities
{
    /// <summary>
    /// TestTransaction class is used to provide transaction level timers when running load tests
    /// </summary>
    public class TestTransaction : IDisposable
    {
        /// <summary>
        /// The test context
        /// </summary>
        private readonly TestContext context;

        /// <summary>
        /// The method name
        /// </summary>
        private readonly string methodName;

        /// <summary>
        /// Has the object been disposed.
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// Has event tracing started.
        /// </summary>
        private bool isStarted;

        /// <summary>
        /// Gets or sets a value indicating whether the outcome is success or failure
        /// </summary>
        public bool Outcome { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestTransaction"/> class.
        /// </summary>
        /// <param name="testContext">The test context.</param>
        /// <param name="actionName">Name of the action.</param>
        public TestTransaction(TestContext testContext, string actionName)
        {
            if (testContext == null)
            {
                throw new ArgumentNullException("testContext");
            }

            this.context = testContext;
            this.Outcome = true;
            this.methodName = testContext.TestName;
            if (actionName != null)
            {
                this.methodName = string.Format("{0}.{1}", testContext.TestName, actionName);
            }
            this.Enter();
        }

        /// <summary>
        /// Enter timeSpan. Invokes starting events.
        /// </summary>
        private void Enter()
        {
            if (!this.isStarted)
            {
                this.context.BeginTimer(this.methodName);
                this.isStarted = true;
            }
        }

        private void Leave()
        {
            if (this.isStarted)
            {
                this.context.EndTimer(this.methodName);
                this.isStarted = false;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Leave();
                this.isDisposed = true;
            }
        }

    }
}
