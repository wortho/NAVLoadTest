using Microsoft.Dynamics.Framework.UI.Client;

namespace Microsoft.Dynamics.Nav.UserSession
{
    /// <summary>
    /// Handles dialog.
    /// </summary>
    public interface IDialogHandler
    {
        /// <summary>
        /// Handles client logical dialog.
        /// </summary>
        /// <param name="dialog">
        /// The dialog.
        /// </param>
        void HandleDialog(ClientLogicalForm dialog);
    }
}