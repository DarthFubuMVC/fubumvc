using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Routing;
using System.Linq;
using FubuCore;
using FubuCore.Util;

namespace FubuMVC.Core.Content
{



    public class FileRouteHandler : IRouteHandler
    {
        private readonly IContentFolderService _folders;
        private readonly ContentType _contentType;

        public FileRouteHandler(IContentFolderService folders, ContentType contentType)
        {
            _folders = folders;
            _contentType = contentType;
        }

        public void RegisterRoute(ICollection<RouteBase> routes)
        {
            var route = new Route("_" + _contentType.ToString(), new RouteValueDictionary(), this);
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
            return new FileHttpHandler(fullPath);
        }
    }

    public class FileHttpHandler : IHttpHandler
    {
        private static readonly Cache<string, string> _mimeTypes = new Cache<string, string>();

        static FileHttpHandler()
        {
            _mimeTypes[".gif"] = "image/gif";
            _mimeTypes[".png"] = "image/png";
            _mimeTypes[".jpg"] = "image/jpeg";
            _mimeTypes[".jpeg"] = "image/jpeg";
            _mimeTypes[".bm"] = "image/bmp";
            _mimeTypes[".bmp"] = "image/bmp";
            _mimeTypes[".css"] = "text/css";
            _mimeTypes[".js"] = "application/x-javascript";
        }

        private readonly string _fileName;

        public FileHttpHandler(string fileName)
        {
            _fileName = fileName;
        }

        public void ProcessRequest(HttpContext context)
        {
            var extension = Path.GetExtension(_fileName).ToLower();

            context.Response.ContentType = _mimeTypes[extension];
            context.Response.WriteFile(_fileName);
        }

        public bool IsReusable
        {
            get { return true; }
        }
    }
}