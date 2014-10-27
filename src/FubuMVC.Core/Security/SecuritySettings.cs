using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Security
{
    [ApplicationLevel]
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
    }
}