using System;
using System.Linq;
using Microsoft.Dynamics.Framework.UI.Client;
using Microsoft.Dynamics.Framework.UI.Client.Interactions;

namespace Microsoft.Dynamics.Nav.UserSession
{
    /// <summary>
    /// Extensions class which interact with specific components such as the CRONUS dialog.
    /// </summary>
    public static class ClientLogicalFormExtensions
    {

        public static ClientRepeaterControl Repeater(this ClientLogicalForm form)
        {
            return form.ContainedControls.OfType<ClientRepeaterControl>().First();
        }

        public static string FindMessage(this ClientLogicalForm form)
        {
            return form.ContainedControls.OfType<ClientStaticStringControl>().First().StringValue;
        }

        public static TType FindLogicalFormControl<TType>(this ClientLogicalForm form, string controlCaption = null)
        {
            return form.ContainedControls.OfType<TType>().First();
        }

        public static void ExecuteQuickFilter(this ClientLogicalForm form, string columnName, string value)
        {
            var filter = form.FindLogicalFormControl<ClientFilterLogicalControl>();
            form.Session.InvokeInteraction(new ExecuteFilterInteraction(filter)
            {
                QuickFilterColumnId = filter.QuickFilterColumns.First(columnDef => columnDef.Caption.Replace("&", "").Equals(columnName)).Id,
                QuickFilterValue = value
            });

            if (filter.ValidationResults.Count > 0)
            {
                throw new ArgumentException("Could not execute filter.");
            }
        }

        /// <summary>
        /// Determines whether [is cronus demo dialog] [the specified dialog].
        /// </summary>
        /// <param name="dialog">The dialog.</param>
        /// <returns>
        ///   <c>true</c> if [is cronus demo dialog] [the specified dialog]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsCronusDemoDialog(ClientLogicalForm dialog)
        {
            if (dialog.IsDialog)
            {
                ClientStaticStringControl staticStringControl = dialog.ContainedControls.OfType<ClientStaticStringControl>().FirstOrDefault();
                if (staticStringControl != null && staticStringControl.StringValue != null)
                {
                    return staticStringControl.StringValue.ToUpperInvariant().Contains("CRONUS");
                }
            }

            return false;
        }

      
    }
}