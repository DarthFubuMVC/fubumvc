using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.Security;

namespace FubuMVC.OwinHost.Middleware.StaticFiles
{
    public interface IStaticFileRule
    {
        AuthorizationRight IsAllowed(IFubuFile file);
    }
}