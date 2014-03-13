using System.Web.Routing;
using FubuCore.Binding;
using FubuCore.Binding.Values;
using FubuMVC.Core.Http.Cookies;

namespace FubuMVC.Core.Http
{
    public class FubuMvcRequestData : RequestData
    {
        public FubuMvcRequestData(IHttpRequest request, RouteData routeData)
        {
            AddValues(new RouteDataValues(routeData));
            AddValues("Querystring", new NamedKeyValues(request.QueryString));
            AddValues("Form", new NamedKeyValues(request.Form));

            AddValues(new CookieValueSource(new Cookies.Cookies(request)));
            AddValues(new HeaderValueSource(request));

            // TODO -- add something back for Files
        }
    }
}