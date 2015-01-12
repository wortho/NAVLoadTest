using Microsoft.VisualStudio.TestTools.LoadTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Dynamics.Nav.TestUtilities
{
    public static class TestContextExtensions
    {
        public static bool IsLoadTestContext(this TestContext testContext)
        {
            return (testContext.Properties.Contains("$LoadTestUserContext"));
        }

        public static LoadTestUserContext GetLoadTestUserContext(this TestContext testContext)
        {
            if (IsLoadTestContext(testContext))
            {
                return testContext.Properties["$LoadTestUserContext"] as LoadTestUserContext;
            }
            return null;
        }
    }
}
