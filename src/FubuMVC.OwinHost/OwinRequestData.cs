using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using FubuCore.Binding;
using FubuCore.Binding.Values;
using FubuCore.Util;
using FubuMVC.Core.Http;

namespace FubuMVC.OwinHost
{
    public class OwinRequestData : RequestData
    {
        public static readonly string Querystring = "OwinQuerystring";
        public static readonly string FormPost = "OwinFormPost";

        public OwinRequestData(RouteData routeData, OwinRequestBody body)
        {
            AddValues(new RouteDataValues(routeData));

            AddValues(Querystring, new DictionaryKeyValues(body.Querystring()));
            AddValues(FormPost, new DictionaryKeyValues(body.FormData ?? new Dictionary<string, string>()));

            AddValues(RequestDataSource.Header.ToString(), new DictionaryKeyValues(body.Headers()));
        }

    }
}