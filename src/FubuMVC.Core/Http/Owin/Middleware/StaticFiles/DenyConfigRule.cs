using System.ComponentModel;
using System.IO;
using FubuCore;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.Security;

namespace FubuMVC.Core.Http.Owin.Middleware.StaticFiles
{
    [Description("Denies read access to any *.config file")]
    public class DenyConfigRule : IStaticFileRule
    {
        public AuthorizationRight IsAllowed(IFubuFile file)
        {
            return Path.GetExtension(file.Path).EqualsIgnoreCase(".config")
                ? AuthorizationRight.Deny
                : AuthorizationRight.None;
        }
    }
}