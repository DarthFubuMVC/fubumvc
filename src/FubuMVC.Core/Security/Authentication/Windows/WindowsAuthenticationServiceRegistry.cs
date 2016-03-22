using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Security.Authentication.Windows
{
    public class WindowsAuthenticationServiceRegistry : ServiceRegistry
    {
        public WindowsAuthenticationServiceRegistry()
        {
            SetServiceIfNone<IWindowsAuthenticationContext, AspNetWindowsAuthenticationContext>();
            SetServiceIfNone<IWindowsPrincipalHandler, DefaultWindowsPrincipalHandler>();
            SetServiceIfNone<IWindowsAuthentication, WindowsAuthentication>();
        }
    }
}