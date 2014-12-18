using System;
using Microsoft.Dynamics.Framework.UI.Client;

namespace Microsoft.Dynamics.Nav.UserSession
{
    /// <summary>
    /// Dialog handler for handling unexpected dialogs.
    /// </summary>
    public class UnexpectedDialogHandler : IDialogHandler
    {
        private readonly Action<ClientLogicalForm> handleDialog;

        /// <summary>
        /// UnexpectedDialogHandlerKey
        /// </summary>
        public const string UnexpectedDialogHandlerKey = "UnexpectedDialogHandler";

        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedDialogHandler"/> class.
        /// </summary>
        /// <param name="handleDialog">
        /// The handle dialog.
        /// </param>
        public UnexpectedDialogHandler(Action<ClientLogicalForm> handleDialog)
        {
            this.handleDialog = handleDialog;
        }

        /// <summary>
        /// Gets or sets a value indicating whether IsSuspended.
        /// </summary>
        public bool IsSuspended { get; set; }

        /// <summary>
        /// Handles client logical dialog.
        /// </summary>
        /// <param name="dialog">
        /// The dialog.
        /// </param>
        public void HandleDialog(ClientLogicalForm dialog)
        {
            if (this.IsSuspended)
            {
                return;
            }

            this.handleDialog(dialog);
        }
    }
}