using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Routing;
using FubuCore;
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

        public OwinRequestData(RouteData routeData, IDictionary<string, object> environment, IDictionary<string, string[]> headers)
        {
            AddValues(new RouteDataValues(routeData));
            AddValues(Querystring, new NamedKeyValues(HttpUtility.ParseQueryString(environment.Get<string>(OwinConstants.RequestQueryStringKey))));
            AddValues(FormPost, new NamedKeyValues(environment.Get<NameValueCollection>(OwinConstants.RequestFormKey)));
            
            var headerValues = headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value));
            AddValues(RequestDataSource.Header.ToString(), new DictionaryKeyValues(headerValues));


        }

    }
}