using System;
using System.Collections.Generic;
using Microsoft.Dynamics.Framework.UI.Client;
using Microsoft.Dynamics.Nav.UserSession;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Dynamics.Nav.TestUtilities
{
    public class NAVTenantUserContextManager : NAVUserContextManager
    {
        private const int NumberOfTenants = 6;
        private const int NumberOfUsersPerTenant = 5;

        public NAVTenantUserContextManager(string navServerUrl, string defaultTenantId, string companyName, int? roleCenterId, string defaultNAVUserName, string defaultNAVPassword) 
            : base(navServerUrl, defaultTenantId, companyName, roleCenterId, defaultNAVUserName, defaultNAVPassword)
        {
        }

        protected override UserContext CreateUserContext(TestContext testContext)
        {
            var userId = GetTestUserId(testContext);
            var userTenantId = GetUserTenantId(userId);
            var userName = GetTenantUserName(userId);
            var userContext = new UserContext(userTenantId, Company, AuthenticationScheme.UserNamePassword, userName, DefaultNAVPassword);
            return userContext;
        }

        private string GetTenantUserName(int userId)
        {
            return userId > 0 ?
                string.Format("{0}{1}", DefaultNAVUserName, userId % NumberOfUsersPerTenant) : DefaultNAVUserName;
        }

        private string GetUserTenantId(int userId)
        {
            return userId > 0 ? 
                string.Format("Cronus{0}", userId % NumberOfTenants) : DefaultTenantId;
        }
    }
}
