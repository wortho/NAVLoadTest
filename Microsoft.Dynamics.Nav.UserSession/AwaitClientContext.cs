using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Dynamics.Framework.UI.Client;

namespace Microsoft.Dynamics.Nav.UserSession
{
    /// <summary>
    /// Helper methods for testing with Client Service
    /// </summary>
    public static partial class ClientSessionExtensions
    {
        /// <summary>
        /// Conext used to await server response.
        /// </summary>
        private class AwaitClientContext : IDisposable
        {
            private readonly AutoResetEvent handle = new AutoResetEvent(false);
            private readonly object handleLock = new object();

            private readonly ClientSession clientSession;

            private readonly EventHandler<ExceptionEventArgs> onUnhandledException;
            private readonly EventHandler<ExceptionEventArgs> onCommunicationError;
            private readonly EventHandler<MessageToShowEventArgs> onInvalidCredentialsError;
            private readonly EventHandler<MessageToShowEventArgs> onMessageToShow;
            private readonly EventHandler onStateChanged;

            public List<Exception> CommunicationErrors { get; private set; }
            public List<Exception> UnhandledExceptions { get; private set; }
            public List<string> Messages { get; private set; }

            public string InvalidCredentialMessage { get; private set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="AwaitClientContext"/> class.
            /// </summary>
            /// <param name="clientSession">The client session.</param>
            /// <param name="readyCondition">The ready condition, which determines when the server is ready.</param>
            public AwaitClientContext(ClientSession clientSession, Func<ClientSession, bool> readyCondition)
            {
                this.clientSession = clientSession;

                this.Messages = new List<string>();
                this.CommunicationErrors = new List<Exception>();
                this.UnhandledExceptions = new List<Exception>();
                this.InvalidCredentialMessage = string.Empty;

                this.onStateChanged = this.ClientSessionOnStateChanged(clientSession, readyCondition);

                this.onMessageToShow = (sender, args) =>
                {
                    this.Messages.Add(args.Message);
                    this.handle.Set();
                };

                this.onUnhandledException = (sender, args) =>
                {
                    this.UnhandledExceptions.Add(args.Exception);
                    this.handle.Set();
                };

                this.onCommunicationError = (sender, args) =>
                {
                    this.CommunicationErrors.Add(args.Exception);
                    this.handle.Set();
                };

                this.onInvalidCredentialsError = (sender, args) =>
                {
                    this.InvalidCredentialMessage = args.Message;
                    this.handle.Set();
                };

                this.clientSession.StateChanged += this.onStateChanged;
                this.clientSession.MessageToShow += this.onMessageToShow;
                this.clientSession.UnhandledException += this.onUnhandledException;
                this.clientSession.CommunicationError += this.onCommunicationError;
                this.clientSession.InvalidCredentialsError += this.onInvalidCredentialsError;
            }

            private EventHandler ClientSessionOnStateChanged(ClientSession session, Func<ClientSession, bool> readyCondition)
            {
                return (sender, args) =>
                {
                    if (readyCondition(session))
                    {
                        if (!this.handle.SafeWaitHandle.IsClosed)
                        {
                            lock (this.handleLock)
                            {
                                if (!this.handle.SafeWaitHandle.IsClosed)
                                {
                                    this.handle.Set();
                                }
                            }
                        }
                    }

                    if (this.UnhandledExceptions.Any() || this.CommunicationErrors.Any())
                    {
                        this.handle.Set();
                    }
                };
            }

            /// <summary>
            /// Waits given number of miliseconds, until ready condition is met or any error is triggered
            /// </summary>
            /// <param name="milisecondsTimeout"></param>
            public void Wait(int milisecondsTimeout)
            {
                handle.WaitOne(milisecondsTimeout);
            }

            public void Dispose()
            {
                this.clientSession.StateChanged -= this.onStateChanged;
                this.clientSession.MessageToShow -= this.onMessageToShow;
                this.clientSession.UnhandledException -= this.onUnhandledException;
                this.clientSession.CommunicationError -= this.onCommunicationError;
                this.clientSession.InvalidCredentialsError -= this.onInvalidCredentialsError;

                lock (this.handleLock)
                {
                    this.handle.Dispose();
                }
            }
        }
    }
}
