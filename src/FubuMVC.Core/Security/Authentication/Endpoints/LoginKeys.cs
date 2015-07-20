using FubuLocalization;

namespace FubuMVC.Core.Security.Authentication.Endpoints
{
    public class LoginKeys : StringToken
    {
        public static readonly LoginKeys LockedOut = new LoginKeys("This user is locked out because of too many failed login attempts. Try back later.");
        public static readonly LoginKeys Failed = new LoginKeys("Incorrect credentials.  Attempt {0} of {1}");
        public static readonly LoginKeys Unknown = new LoginKeys("The credentials supplied are incorrect");
        public static readonly LoginKeys Login = new LoginKeys("Login");

        public static readonly LoginKeys LoginWithWindows = new LoginKeys("Login with Windows");

        public static readonly LoginKeys YourUsername = new LoginKeys("Your email...");
        public static readonly LoginKeys YourPassword = new LoginKeys("Your password...");
        public static readonly LoginKeys RememberMe = new LoginKeys("Remember me on this device");
		public static readonly LoginKeys ForgotPassword = new LoginKeys("Forgot your password?");
		public static readonly LoginKeys ResetPassword = new LoginKeys("Reset it here");

        protected LoginKeys(string defaultValue)
            : base(null, defaultValue, namespaceByType: true)
        {
        }
    }
}