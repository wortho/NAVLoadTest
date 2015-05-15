using System.Collections.Generic;
using Microsoft.Dynamics.Framework.UI.Client;
using Microsoft.Dynamics.Nav.UserSession;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.Dynamics.Nav.TestUtilities
{
    public class NAVTenantUserContextManager : NAVUserContextManager
    {
        private static readonly Dictionary<string, string> UserTenantMap = new Dictionary<string, string>
        {
            {"User", "default"},
            {"User0", "default"},
            {"User1", "Cronus1"},
            {"User2", "Cronus1"},
            {"User3", "Cronus2"},
            {"User4", "Cronus2"},
            {"User5", "Cronus3"},
            {"User6", "Cronus3"},
            {"User7", "Cronus4"},
            {"User8", "Cronus4"},
            {"User9", "Cronus5"},
            {"User10", "Cronus5"}
        };

        public NAVTenantUserContextManager(string navServerUrl, string defaultTenantId, string companyName, int? roleCenterId, string defaultNAVUserName, string defaultNAVPassword) 
            : base(navServerUrl, defaultTenantId, companyName, roleCenterId, defaultNAVUserName, defaultNAVPassword)
        {
        }

        protected override UserContext CreateUserContext(TestContext testContext)
        {
            var userName = GetUserName(testContext);
            var userTenantId = GetUserTenantId(userName);
            var userContext = new UserContext(userTenantId, Company, AuthenticationScheme.UserNamePassword, userName, DefaultNAVPassword);
            return userContext;
        }

        private string GetUserTenantId(string userName)
        {
            string tenantId;
            return UserTenantMap.TryGetValue(userName, out tenantId) ? 
                tenantId :
                DefaultTenantId;
        }
    }
}
