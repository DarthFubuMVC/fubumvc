using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using FubuCore;
using FubuMVC.Core.Assets.Files;

namespace FubuMVC.Core.Assets.Http
{
    public class AssetContentHandler : IRouteHandler
    {
        public static readonly string AssetsUrlFolder = "_content";
        private readonly Func<IContentWriter> _writerSource;

        public AssetContentHandler(Func<IContentWriter> writerSource)
        {
            _writerSource = writerSource;
        }

        public Route BuildRoute()
        {
            var route = new Route(AssetsUrlFolder, new RouteValueDictionary(), this);
            for (int i = 0; i < 10; i++)
            {
                route.Url += "/{" + i + "}";
                route.Defaults.Add(i.ToString(), string.Empty);
            }

            return route;
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var writer = _writerSource();
            var routeValues = requestContext.RouteData.Values.Select(x => x.Value as string).Where(x => x.IsNotEmpty());

            return new AssetHttpHandler(writer, routeValues);
        }


    }
}