using System.Diagnostics;

namespace FubuMVC.Core.Security.Authorization
{
    public class SecuritySettings
    {
        public SecuritySettings()
        {
            Reset();
        }

        public bool AuthorizationEnabled { get; set; }
        public bool AuthenticationEnabled { get; set; }

        public void Reset()
        {
            AuthorizationEnabled = AuthenticationEnabled = true;
        }

        public void Disable()
        {
            AuthenticationEnabled = AuthorizationEnabled = false;
        }
    }
}