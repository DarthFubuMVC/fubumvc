using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Routing;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Assets;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Content
{
    [MarkedForTermination("will be obsolete with AssetFileHandler")]
    public class FileRouteHandler : IRouteHandler
    {

        public void RegisterRoute(ICollection<RouteBase> routes)
        {
            throw new NotImplementedException();
            //var route = new Route("_content/" + _contentType.ToString(), new RouteValueDictionary(), this);
            //for (int i = 0; i < 10; i++)
            //{
            //    route.Url += "/{" + i + "}";
            //    route.Defaults.Add(i.ToString(), string.Empty);
            //}

            //routes.Add(route);
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            throw new NotImplementedException();
            //var fileName = requestContext.RouteData.Values.Select(x => x.Value as string).Where(x => x.IsNotEmpty()).Join("/");
            //var fullPath = _folders.FileNameFor(_contentType, fileName);
            //return new FileHttpHandler(fullPath, _mimeTypeProvider);
        }
    }

    //[MarkedForTermination("will be obsolete with AssetFileHandler")]
    //public class FileHttpHandler : IHttpHandler
    //{
    //    private readonly string _fileName;
    //    private readonly IMimeTypeProvider _mimeTypeProvider;

    //    public FileHttpHandler(string fileName, IMimeTypeProvider mimeTypeProvider)
    //    {
    //        _fileName = fileName;
    //        _mimeTypeProvider = mimeTypeProvider;
    //    }

    //    public void ProcessRequest(HttpContext context)
    //    {
    //        var extension = Path.GetExtension(_fileName).ToLower();

    //        context.Response.ContentType = _mimeTypeProvider.For(extension).Value;
    //        context.Response.WriteFile(_fileName);
    //    }

    //    public bool IsReusable
    //    {
    //        get { return true; }
    //    }
    //}
}