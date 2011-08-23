using System;
using System.Web;
using System.Web.Routing;
using FubuCore;

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
            var url = "{0}/{1}/{2}".ToFormat(AssetsUrlFolder, subject.Folder, subject.Name);
            return url.ToAbsoluteUrl();
        }
    }
}