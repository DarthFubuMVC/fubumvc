using System.Web;
using System.Web.Routing;

namespace Spark.Web.FubuMVC
{
    public class ControllerContext
    {
        private readonly object _controller;
        private readonly HttpContextBase _httpContext;
        private readonly RouteData _routeData;

        public ControllerContext(HttpContextBase httpContext, RouteData routeData, object controller)
        {
            _httpContext = httpContext;
            _routeData = routeData;
            _controller = controller;
        }

        public RouteData RouteData
        {
            get { return _routeData; }
        }

        public object Controller
        {
            get { return _controller; }
        }
    }
}