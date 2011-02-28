using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Routing;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core.Content
{
    public class FileRouteHandler : IRouteHandler
    {
        private readonly IContentFolderService _folders;
        private readonly ContentType _contentType;
        private readonly IMimeTypeProvider _mimeTypeProvider;

        public FileRouteHandler(IContentFolderService folders, ContentType contentType, IMimeTypeProvider mimeTypeProvider)
        {
            _folders = folders;
            _contentType = contentType;
            _mimeTypeProvider = mimeTypeProvider;
        }

        public void RegisterRoute(ICollection<RouteBase> routes)
        {
            var route = new Route("_content/" + _contentType.ToString(), new RouteValueDictionary(), this);
            for (int i = 0; i < 10; i++)
            {
                route.Url += "/{" + i + "}";
                route.Defaults.Add(i.ToString(), string.Empty);
            }

            routes.Add(route);
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var fileName = requestContext.RouteData.Values.Select(x => x.Value as string).Where(x => x.IsNotEmpty()).Join("/");
            var fullPath = _folders.FileNameFor(_contentType, fileName);
            return new FileHttpHandler(fullPath, _mimeTypeProvider);
        }
    }

    public class FileHttpHandler : IHttpHandler
    {
        private readonly string _fileName;
        private readonly IMimeTypeProvider _mimeTypeProvider;

        public FileHttpHandler(string fileName, IMimeTypeProvider mimeTypeProvider)
        {
            _fileName = fileName;
            _mimeTypeProvider = mimeTypeProvider;
        }

        public void ProcessRequest(HttpContext context)
        {
            var extension = Path.GetExtension(_fileName).ToLower();

            context.Response.ContentType = _mimeTypeProvider.For(extension);
            context.Response.WriteFile(_fileName);
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}