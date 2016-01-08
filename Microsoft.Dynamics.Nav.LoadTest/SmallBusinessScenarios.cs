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
        private const int PostedPurchaseInvoiceCard = 1357;
        private const int MiniVendorList = 1331;
        private const int PostedPurchaseInvoiceList = 1359;

        private static UserContextManager userContextManager;

        public UserContextManager UserContextManager
        {
            get { return userContextManager ?? CreateUserContextManager(); }
        }

        private static UserContextManager CreateUserContextManager()
        {
            // use Windows authentication
            userContextManager = new WindowsUserContextManager(
                   Settings.Default.NAVClientService,
                   null,
                   null,
                   SmallBusinessRoleCentre);
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
        public void CreateAndPostPurchaseInvoice()
        {
            // Create a new Purchase Invoice
            TestScenario.Run(
                UserContextManager,
                TestContext,
                userContext =>
                {
                    var newPurchaseInvoicePage = CreateNewPurchaseInvoice(userContext);
                    PostPurchaseInvoice(userContext, newPurchaseInvoicePage);
                });
        }

        private ClientLogicalForm CreateNewPurchaseInvoice(UserContext userContext)
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

            var vendorInvoiceNo = SafeRandom.GetRandomNext(100000, 999999);
            TestScenario.SaveValueWithDelay(
                newPurchaseInvoicePage.Control("Vendor Invoice No."),
                vendorInvoiceNo);

            // Add a random number of lines between 2 and 15
            var noOfLines = SafeRandom.GetRandomNext(2, 15);
            for (var line = 0; line < noOfLines; line++)
            {
                AddPurchaseInvoiceLine(userContext, newPurchaseInvoicePage, line);
            }

            userContext.ValidateForm(newPurchaseInvoicePage);
            TestContext.WriteLine(
                "Created Purchase Invoice {0}",
                newPurchaseInvoicePage.Caption);
            return newPurchaseInvoicePage;
        }


        private void AddPurchaseInvoiceLine(
            UserContext userContext,
            ClientLogicalForm purchaseInvoicePage,
            int index)
        {
            using (new TestTransaction(TestContext, "AddPurchaseInvoiceLine"))
            {
                var repeater = purchaseInvoicePage.Repeater();
                var rowCount = repeater.Offset + repeater.DefaultViewport.Count;
                if (index >= rowCount)
                {
                    // scroll to the next viewport
                    userContext.InvokeInteraction(
                        new ScrollRepeaterInteraction(repeater, 1));
                }

                var rowIndex = (int) (index - repeater.Offset);
                var itemsLine = repeater.DefaultViewport[rowIndex];

                // select random Item No. from  lookup
                var itemNoControl = itemsLine.Control("Item No.");
                var itemNo = TestScenario.SelectRandomRecordFromLookup(
                    TestContext,
                    userContext,
                    itemNoControl,
                    "No.");
                TestScenario.SaveValueWithDelay(itemNoControl, itemNo);

                var qtyToOrder = SafeRandom.GetRandomNext(1, 10);
                TestScenario.SaveValueWithDelay(itemsLine.Control("Quantity"), qtyToOrder);
            }
        }

        private void PostPurchaseInvoice(
            UserContext userContext,
            ClientLogicalForm purchaseInvoicePage)
        {
            ClientLogicalForm openPostedInvoiceDialog;
            using (new TestTransaction(TestContext, "Post"))
            {
                var postConfirmationDialog = purchaseInvoicePage.Action("Post")
                    .InvokeCatchDialog();
                if (postConfirmationDialog == null)
                {
                    userContext.ValidateForm(purchaseInvoicePage);
                    Assert.Fail("Confirm Post dialog not found");
                }
                openPostedInvoiceDialog = postConfirmationDialog.Action("Yes")
                    .InvokeCatchDialog();
            }

            if (openPostedInvoiceDialog == null)
            {
                Assert.Fail("Open Posted Invoice dialog not found");
            }

            ClientLogicalForm postedPurchaseInvoicePage;
            using (new TestTransaction(TestContext, "OpenPostedPurchaseInvoice"))
            {
                postedPurchaseInvoicePage = userContext.EnsurePage(
                        PostedPurchaseInvoiceCard,
                        openPostedInvoiceDialog.Action("Yes").InvokeCatchForm());

            }

            TestContext.WriteLine(
                    "Posted Purchase Invoice {0}",
                    postedPurchaseInvoicePage.Caption);

            TestScenario.ClosePage(
                TestContext,
                userContext,
                postedPurchaseInvoicePage);
        }

        [TestMethod]
        public void SortPostedPurchaseInvoiceListByAmount()
        {
            TestScenario.Run(
                UserContextManager,
                TestContext,
                userContext =>
                {
                    TestScenario.RunPageAction(
                        TestContext,
                        userContext,
                        PostedPurchaseInvoiceList,
                        form =>
                        {
                            var amountColumnControl = form.Repeater().Column("Amount");
                            using (new TestTransaction(TestContext, "SortAmountDescending"))
                            {
                                userContext.InvokeInteraction(
                                    new InvokeActionInteraction(
                                        amountColumnControl.Action("Descending")));
                            }
                            using (new TestTransaction(TestContext, "SortAmountAscending"))
                            {
                                userContext.InvokeInteraction(
                                    new InvokeActionInteraction(
                                        amountColumnControl.Action("Ascending")));
                            }
                        });
                });
        }

        [TestMethod]
        public void FilterPostedPurchaseInvoiceListByVendor()
        {
            TestScenario.Run(
                UserContextManager,
                TestContext,
                userContext =>
                {
                    // select a random vendor to filter by
                    var vendorName = TestScenario.SelectRandomRecordFromListPage(
                        TestContext,
                        userContext,
                        MiniVendorList,
                        "Name");

                    TestScenario.RunPageAction(
                        TestContext,
                        userContext,
                        PostedPurchaseInvoiceList,
                        form =>
                        {
                            var vendorNameColumn = form.Repeater().Column("Vendor Name");
                            TestScenario.ApplyColumnFilter(
                                TestContext,
                                userContext,
                                vendorNameColumn,
                                vendorName);
                            using (new TestTransaction(TestContext, "ClearFilterByVendorName"))
                            {
                                userContext.InvokeInteraction(
                                    new InvokeActionInteraction(
                                        vendorNameColumn.Action("Clear Filter")));
                            }
                        });
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
