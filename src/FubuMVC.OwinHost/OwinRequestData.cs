using System.Collections.Generic;
using System.Web.Routing;
using FubuCore.Binding;
using FubuCore.Util;
using FubuMVC.Core.Http;

namespace FubuMVC.OwinHost
{
    public class OwinRequestData : RequestData
    {
        public OwinRequestData(RouteData routeData, OwinRequestBody body)
        {
            AddValues(new RouteDataValues(routeData));

            AddValues("OwinQuerystring", new DictionaryKeyValues(body.Querystring()));
            AddValues("OwinFormPost", new DictionaryKeyValues(body.FormData ?? new Dictionary<string, string>()));

            AddValues(RequestDataSource.Header.ToString(), new DictionaryKeyValues(body.Headers()));
        }
    }
}