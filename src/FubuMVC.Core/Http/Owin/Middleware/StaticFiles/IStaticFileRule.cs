using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.Security;
using FubuMVC.Core.Security.Authorization;

namespace FubuMVC.Core.Http.Owin.Middleware.StaticFiles
{
    public interface IStaticFileRule
    {
        AuthorizationRight IsAllowed(IFubuFile file);
    }
}