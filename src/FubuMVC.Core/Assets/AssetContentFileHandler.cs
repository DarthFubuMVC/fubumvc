using System;
using System.Web;
using System.Web.Routing;
using FubuCore;
using FubuMVC.Core.Assets.Content;
using FubuMVC.Core.Assets.Files;

namespace FubuMVC.Core.Assets
{
    public class AssetContentFileHandler : IRouteHandler
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

    public class ContentPlanHandler : IHttpHandler
    {
        private readonly ContentPlan _plan;

        public ContentPlanHandler(ContentPlan plan)
        {
            _plan = plan;
        }

        public void ProcessRequest(HttpContext context)
        {
            throw new NotImplementedException();

            //var response = context.Response;
            
            //response.AddFileDependencies(_plan.AllFileNames());

            //setCachePolicies(response.Cache);

            // need to write with MimeType, encoding.
        }

        private void setCachePolicies(HttpCachePolicy cache)
        {
            cache.VaryByParams["files"] = true;
            cache.SetLastModifiedFromFileDependencies();
            cache.SetETagFromFileDependencies();
            cache.SetCacheability(HttpCacheability.Public);
        }

        // These handlers CANNOT be reused
        public bool IsReusable
        {
            get { return false; }
        }
    }
}