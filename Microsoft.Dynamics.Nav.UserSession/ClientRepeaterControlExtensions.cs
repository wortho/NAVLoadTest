using System;
using System.Linq;
using Microsoft.Dynamics.Framework.UI.Client;

namespace Microsoft.Dynamics.Nav.UserSession
{
    public static class ClientRepeaterControlExtensions
    {
        public static ClientRepeaterColumnControl Column(
            this ClientRepeaterControl repeater,
            string columnCaption)
        {
            try
            {
                return repeater.Columns.First(c => c.Caption == columnCaption);
            }
            catch (InvalidOperationException exception)
            {
                repeater.WriteControlCaptions<ClientActionControl>();
                throw new ArgumentOutOfRangeException(
                    string.Format(
                        "Could not find an Column with caption: {0}",
                        columnCaption),
                    exception);
            }
        }

    }
}
