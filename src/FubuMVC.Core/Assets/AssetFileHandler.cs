using System;
using System.Web;
using System.Web.Routing;
using FubuCore;
using FubuMVC.Core.Assets.Files;

namespace FubuMVC.Core.Assets
{
    public class AssetFileHandler : IRouteHandler
    {
        public static readonly string AssetsUrlFolder = "_content";

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            throw new NotImplementedException();
        }

        public static string DetermineAssetUrl(IAssetTagSubject subject)
        {
            var folder = subject.Folder;
            var name = subject.Name;

            return DetermineAssetUrl(folder, name);
        }

        public static string DetermineAssetUrl(AssetFolder? folder, string name)
        {
            var url = "{0}/{1}/{2}".ToFormat(AssetsUrlFolder, folder, name);
            return url.ToAbsoluteUrl();
        }
    }
}