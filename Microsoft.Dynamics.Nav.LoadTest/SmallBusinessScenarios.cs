using System.Globalization;
using Microsoft.Dynamics.Framework.UI.Client;
using Microsoft.Dynamics.Framework.UI.Client.Interactions;
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
                            TestContext.WriteLine(
                                "Page Caption {0}",
                                form.Caption);
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
                    var newPurchaseInvoicePage = userContext.EnsurePage(
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
                        newPurchaseInvoicePage.Control("Vendor Name"),
                        vendorName);

                    TestScenario.SaveValueWithDelay(
                        newPurchaseInvoicePage.Control("Vendor Invoice No."),
                        "999999");

                    // Add a random number of lines between 2 and 15
                    var noOfLines = SafeRandom.GetRandomNext(2, 15);
                    for (var line = 0; line < noOfLines; line++)
                    {
                        AddPurchaseLine(userContext, newPurchaseInvoicePage, line);
                    }

                    userContext.ValidateForm(newPurchaseInvoicePage);
                    TestContext.WriteLine(
                        "Created Purchase Invoice {0}",
                        newPurchaseInvoicePage.Caption);
                    TestScenario.ClosePage(
                        TestContext,
                        userContext,
                        newPurchaseInvoicePage);
                });
        }


        private void AddPurchaseLine(UserContext userContext, ClientLogicalForm purchaseInvoicePage, int index)
        {
            var repeater = purchaseInvoicePage.Repeater();
            var rowCount = repeater.Offset + repeater.DefaultViewport.Count;
            if (index >= rowCount)
            {
                // scroll to the next viewport
                userContext.InvokeInteraction(new ScrollRepeaterInteraction(repeater, 1));
            }

            var rowIndex = (int)(index - repeater.Offset);
            var itemsLine = repeater.DefaultViewport[rowIndex];
            
            // select random Item No. from  lookup
            var itemNoControl = itemsLine.Control("Item No.");
            var itemNo = TestScenario.SelectRandomRecordFromLookup(TestContext, userContext, itemNoControl, "No.");
            TestScenario.SaveValueWithDelay(itemNoControl, itemNo);

            var qtyToOrder = SafeRandom.GetRandomNext(1, 10).ToString(CultureInfo.InvariantCulture);
            TestScenario.SaveValueWithDelay(itemsLine.Control("Quantity"), qtyToOrder);
        }

        [ClassCleanup]
        public static void Cleanup()
        {
            if (userContextManager != null)
                userContextManager.CloseAllSessions();
        }
    }
}
