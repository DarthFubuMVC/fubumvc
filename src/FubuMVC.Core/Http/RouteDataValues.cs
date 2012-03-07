using System.Web.Routing;
using FubuCore.Binding.Values;

namespace FubuMVC.Core.Http
{
    public class RouteDataValues : GenericValueSource
    {
        public RouteDataValues(RouteData data)
            : base(RequestDataSource.Route.ToString(), key => data.Values[key], () => data.Values.Keys)
        {
        }
    }
}