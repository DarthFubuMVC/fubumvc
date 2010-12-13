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
    public class ImageRouteHandler : IRouteHandler
    {
        private readonly PackagedImageUrlResolver _resolver;

        public ImageRouteHandler(PackagedImageUrlResolver resolver)
        {
            _resolver = resolver;
        }

        public void RegisterRoute(ICollection<RouteBase> routes)
        {
            var route = new Route("_images", new RouteValueDictionary(), this);
            for (int i = 0; i < 10; i++)
            {
                route.Url += "/{" + i + "}";
                route.Defaults.Add(i.ToString(), string.Empty);
            }

            routes.Add(route);
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var imageName = requestContext.RouteData.Values.Select(x => x.Value as string).Where(x => x.IsNotEmpty()).Join("/");
            var fileName = _resolver.FileNameFor(imageName);
            return new ImageHttpHandler(fileName);
        }
    }

    public class ImageHttpHandler : IHttpHandler
    {
        private static Cache<string, string> _mimeTypes = new Cache<string, string>();

        static ImageHttpHandler()
        {
            _mimeTypes[".gif"] = "image/gif";
            _mimeTypes[".png"] = "image/png";
            _mimeTypes[".jpg"] = "image/jpeg";
            _mimeTypes[".jpeg"] = "image/jpeg";
            _mimeTypes[".bm"] = "image/bmp";
            _mimeTypes[".bmp"] = "image/bmp";
        }

        private readonly string _fileName;

        public ImageHttpHandler(string fileName)
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