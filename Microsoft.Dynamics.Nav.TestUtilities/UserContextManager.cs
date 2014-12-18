using System;
using System.Collections.Concurrent;
using Microsoft.Dynamics.Nav.UserSession;

namespace Microsoft.Dynamics.Nav.TestUtilities
{
    /// <summary>
    /// Delegate to enable the test to create a user context
    /// </summary>
    /// <param name="tenantId">tenantId for the user</param>
    /// <param name="company">company for the user</param>
    /// <returns></returns>
    public delegate UserContext CreateUserContextDelegate(string tenantId, string company);

    /// <summary>
    /// UserContextManager manges a limited pool of user contexts for a given tenant/company
    /// </summary>
    public class UserContextManager : IDisposable
    {
        private readonly CreateUserContextDelegate createUserContext;
        private ConcurrentQueue<UserContext> UserContextPool { get; set; }
        public int UserContextsPerUserContextManager { get; private set; }
        public string TenantId { get; private set; }
        public string Company { get; private set; }

        public UserContextManager(string tenantId, string companyName, CreateUserContextDelegate createUserContext)
        {
            if (createUserContext == null)
            {
                throw new ArgumentNullException("createUserContext");
            }
            this.UserContextPool = new ConcurrentQueue<UserContext>();
            this.createUserContext = createUserContext;
            this.TenantId = tenantId;
            this.Company = companyName;
            this.UserContextsPerUserContextManager = 10;
        }


        public UserContext GetUserContext()
        {
            if (this.UserContextsPerUserContextManager > 0)
            {
                this.UserContextsPerUserContextManager--;
            }
            else
            {
                if (this.UserContextPool.Count > 0)
                {
                    UserContext userContext;
                    if (this.UserContextPool.TryDequeue(out userContext))
                    {
                        return userContext;
                    }
                }
            }
            return createUserContext(this.TenantId, this.Company);
        }


        public void ReturnUserContext(UserContext userContext)
        {
            if (userContext != null)
            {
                this.UserContextPool.Enqueue(userContext);
            }
        }

        public void Dispose()
        {
            CloseAllSessions();
        }

        public void CloseAllSessions()
        {
            UserContext userContext;
            // close any open sessions
            while (this.UserContextPool.TryDequeue(out userContext))
            {
                userContext.CloseSession();
            }
        }
    }
}
