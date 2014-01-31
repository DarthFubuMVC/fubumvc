using System.ComponentModel;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.Security;

namespace FubuMVC.OwinHost.Middleware.StaticFiles
{
    [Description("Allow read access to javascript, css, image, and html files")]
    public class AssetStaticFileRule : IStaticFileRule
    {
        public AuthorizationRight IsAllowed(IFubuFile file)
        {
            var mimetype = MimeType.MimeTypeByFileName(file.Path);
            if (mimetype == null) return AuthorizationRight.None;

            if (mimetype == MimeType.Javascript) return AuthorizationRight.Allow;

            if (mimetype == MimeType.Css) return AuthorizationRight.Allow;

            if (mimetype == MimeType.Html) return AuthorizationRight.Allow;

            if (mimetype.Value.StartsWith("image/")) return AuthorizationRight.Allow;

            return AuthorizationRight.None;
        }
    }
}