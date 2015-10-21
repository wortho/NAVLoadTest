using Microsoft.Dynamics.Nav.LoadTest.Properties;
using Microsoft.Dynamics.Nav.TestUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Dynamics.Nav.UserSession;

namespace Microsoft.Dynamics.Nav.LoadTest
{
    [TestClass]
    public class SmallBusinessScenarios
    {
        public TestContext TestContext { get; set; }

        private const int SmallBusinessRoleCentre = 9022;
        private const int MiniPurchaseInvoiceList = 1356;
        private const int MiniPurchaseInvoiceCard = 1354;
        private const int MiniVendorList = 1331;

        private static UserContextManager userContextManager;

        public UserContextManager UserContextManager
        {
            get { return userContextManager ?? CreateUserContextManager(); }
        }

        private static UserContextManager CreateUserContextManager()
        {
            // use NAV User Password authentication
            userContextManager = new NAVUserContextManager(
                   Settings.Default.NAVClientService,
                   null,
                   null,
                   SmallBusinessRoleCentre,
                   Settings.Default.NAVUserName,
                   Settings.Default.NAVUserPassword);
            return userContextManager;
        }
        

        [TestMethod]
        public void OpenCloseMiniPurchaseInvoiceList()
        {
            // Open and Close MiniPurchaseInvoiceList
            TestScenario.Run(
                UserContextManager,
                TestContext,
                userContext =>
                {
                    TestScenario.RunPageAction(
                        TestContext,
                        userContext,
                        MiniPurchaseInvoiceList,
                        form =>
                        {
                            TestContext.WriteLine("Page Caption {0}", form.Caption);
                        });
                });
        }

        [TestMethod]
        public void CreateNewPurchaseOrder()
        {
            // Create a new Purchase Order
            TestScenario.Run(
                UserContextManager,
                TestContext,
                userContext =>
                {
                    // Invoke using the Purchase Invoice action on Role Center and catch the new page
                    var newPurchaseInvoiceForm = userContext.EnsurePage(
                        MiniPurchaseInvoiceCard,
                        userContext.RoleCenterPage.Action("Purchase Invoice")
                            .InvokeCatchForm());

                    var vendorName = TestScenario.SelectRandomRecordFromListPage(
                        TestContext,
                        userContext,
                        MiniVendorList,
                        "Name");

                    TestScenario.SaveValueAndIgnoreWarning(
                        TestContext,
                        userContext,
                        newPurchaseInvoiceForm.Control("Vendor Name"),
                        vendorName);

                    TestContext.WriteLine("Created Purchase Invoice {0}", newPurchaseInvoiceForm.Caption);

                    TestScenario.ClosePage(TestContext, userContext, newPurchaseInvoiceForm);
                });
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            if (userContextManager != null)
                userContextManager.CloseAllSessions();
        }
    }
}
