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
using FubuMVC.Core.Http.Cookies;

namespace FubuMVC.OwinHost
{
    public class OwinRequestData : RequestData
    {
        public static readonly string Querystring = "OwinQuerystring";
        public static readonly string FormPost = "OwinFormPost";

        public OwinRequestData(RouteData routeData, IDictionary<string, object> environment, ICurrentHttpRequest currentRequest)
        {
            AddValues(new RouteDataValues(routeData));
            AddValues(Querystring, new NamedKeyValues(HttpUtility.ParseQueryString(environment.Get<string>(OwinConstants.RequestQueryStringKey))));
            AddValues(FormPost, new NamedKeyValues(environment.Get<NameValueCollection>(OwinConstants.RequestFormKey)));

            AddValues(new CookieValueSource(new Cookies(currentRequest)));
            AddValues(new HeaderValueSource(currentRequest));
        }

    }
}