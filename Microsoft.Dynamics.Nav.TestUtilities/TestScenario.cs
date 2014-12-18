using System;
using System.Globalization;
using Microsoft.Dynamics.Framework.UI.Client;
using Microsoft.Dynamics.Framework.UI.Client.Interactions;
using Microsoft.Dynamics.Nav.UserSession;
using Microsoft.VisualStudio.TestTools.LoadTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Dynamics.Nav.TestUtilities
{
    public class TestScenario
    {
        /// <summary>
        /// Thread safety for UserContext creation
        /// </summary>
        private static readonly object Lockobj = new object();

        /// <summary>
        /// Delegate for creating a new UserContext
        /// </summary>
        /// <param name="testContext">The current Test Context</param>
        /// <param name="tenantId">Tenant to use or null if no tenant pre-selected</param>
        /// <param name="company">Company to use or null if no company pre-selected</param>
        /// <param name="roleCenterId">Role Center to use for the user</param>
        /// <param name="navServerUrl"></param>
        /// <param name="scheme">Authentication Scheme</param>
        /// <param name="navUsername">User Name</param>
        /// <param name="navPassword">Password</param>
        /// <returns>a new UserContext</returns>
        public static UserContext CreateUserContext(TestContext testContext, string tenantId, string company, int? roleCenterId, string navServerUrl, AuthenticationScheme scheme, string navUsername = null, string navPassword = null)
        {
            lock (Lockobj)
            {
                if (tenantId == null)
                {
                    //TODO Select default tenant if running multiple tenants
                }
                if (company == null)
                {
                    //TODO: Select default company if running multiple companies
                }

                var userContext = new UserContext(tenantId, company);
                using (new TestTransaction(testContext, "OpenSession"))
                {
                    userContext.InitializeSession(navServerUrl, tenantId, company, scheme, navUsername, navPassword);
                    userContext.OpenSession();
                }

                if (roleCenterId.HasValue)
                {
                    using (new TestTransaction(testContext, "OpenRoleCenter"))
                    {
                        userContext.OpenRoleCenter(roleCenterId.Value);
                    }
                }

                return userContext;
            }
        }

        /// <summary>
        /// Run an end-to-end test scenario
        /// Ensure any opened forms are closed at the end of the scenario
        /// </summary>
        /// <param name="manager">UserContextManager</param>
        /// <param name="testContext">current Test Context</param>
        /// <param name="action">function to perform the user actions</param>
        /// <param name="actionName">the name of the scenario being performed</param>
        public static void Run(UserContextManager manager, TestContext testContext, Action<UserContext> action, string actionName = null)
        {
            var userContext = manager.GetUserContext();
            var formCount = userContext.GetOpenFormCount();
            try
            {
                action(userContext);
                userContext.CheckOpenForms(formCount);
                userContext.WaitForReady();
                manager.ReturnUserContext(userContext);
            }
            catch (Exception)
            {
                // if error occurs we close the session and don't return to the pool
                CloseSession(userContext, testContext);
                throw;
            }
        }

        /// <summary>
        /// Open a page run an action and close the page
        /// </summary>
        /// <param name="testContext">current Test Context</param>
        /// <param name="context">current user Context</param>
        /// <param name="pageId">page to open</param>
        /// <param name="formAction">action to run</param>
        public static void RunPageAction(TestContext testContext, UserContext context, int pageId, Action<ClientLogicalForm> formAction = null)
        {
            var form = OpenPage(testContext, context, pageId);

            if (formAction != null)
            {
                formAction(form);
            }

            ClosePage(testContext, context, form);
        }

        public static ClientLogicalForm OpenPage(TestContext testContext, UserContext context, int pageId)
        {
            ClientLogicalForm form;
            using (new TestTransaction(testContext, String.Format("OpenPage{0}", pageId)))
            {
                form = context.OpenForm(pageId.ToString(CultureInfo.InvariantCulture));
                context.EnsurePage(pageId, form);
            }

            DelayTiming.SleepDelay(DelayTiming.OpenFormDelay);
            return form;
        }

        public static void ClosePage(TestContext testContext, UserContext context, ClientLogicalForm form)
        {
            using (new TestTransaction(testContext, String.Format("ClosePage{0}", UserContext.GetActualPageNo(form))))
            {
                context.InvokeInteraction(new CloseFormInteraction(form));
            }
        }
        private static void CloseSession(UserContext userContext, TestContext testContext)
        {
            try
            {
                using (new TestTransaction(testContext, "CloseSession"))
                {
                    userContext.CloseSession();
                }
            }
            catch (Exception exception)
            {
                testContext.WriteLine("Error occurred on CloseSession {0}", exception.Message);
            }
        }


        public static void SaveValueWithDelay(ClientLogicalControl control, object value)
        {
            control.SaveValue(value);
            DelayTiming.SleepDelay(DelayTiming.EntryDelay);
        }

        /// <summary>
        /// Save the specified value with entry delay and ignore any warning dialogs
        /// </summary>
        /// <param name="context">the current test contest</param>
        /// <param name="userContext">the user context</param>
        /// <param name="control">the control to be updated</param>
        /// <param name="value">the value</param>
        /// <param name="ignoreAction">the name of the action, default is Yes</param>
        public static void SaveValueAndIgnoreWarning(TestContext context, UserContext userContext, ClientLogicalControl control, string value, string ignoreAction = "Yes")
        {
            var dialog = userContext.CatchDialog(() => SaveValueWithDelay(control, value));
            if (dialog != null)
            {
                try
                {
                    var action = dialog.Action(ignoreAction);
                    if (action != null)
                    {
                        action.Invoke();
                        context.WriteLine("Dialog Caption: {0} Message: {1} was ignored with Action: {2} ", dialog.Caption, dialog.FindMessage(), ignoreAction);
                    }
                }
                catch (InvalidOperationException)
                {
                    context.WriteLine("Dialog Caption: {0} Message: {1} Action: {2} was not found.", dialog.Caption, dialog.FindMessage(), ignoreAction);
                    throw;
                }

            }
        }
        
        public static string GetUserNameFromTestContext(TestContext testContext)
        {
            LoadTestUserContext loadTestUserContext = GetLoadTestUserContext(testContext);
            if (loadTestUserContext != null)
            {
                return String.Format("User{0}", loadTestUserContext.UserId);
            }

            // empty user name will use the configuration user name
            return String.Empty;

        }

        private static LoadTestUserContext GetLoadTestUserContext(TestContext testContext)
        {
            if (testContext.Properties.Contains("$LoadTestUserContext"))
            {
                return testContext.Properties["$LoadTestUserContext"] as LoadTestUserContext;
            }
            return null;
        }

        public static string SelectRandomRecordFromListPage(TestContext testContext, int pageId, UserContext context, string keyFieldCaption)
        {
            string randomKey = null;
            RunPageAction(testContext, context, pageId, form =>
            {
                // selects a random row from the first page of results
                int rowCount = form.Repeater().DefaultViewport.Count;
                int rowToSelect = SafeRandom.GetRandomNext(rowCount);
                var rowControl = form.Repeater().DefaultViewport[rowToSelect];
                randomKey = rowControl.Control(keyFieldCaption).StringValue;
                testContext.WriteLine("Selected Random Record Page:{0} Key:{1} Value:{2}", pageId, keyFieldCaption, randomKey);
            });
            return randomKey;
        }
    }
}
