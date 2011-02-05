using System.Web.Routing;

namespace Spark.Web.FubuMVC
{
    public class ActionContext
    {
        public ActionContext(RouteData routeData, string actionNamespace, string actionName)
        {
            RouteData = routeData;
            ActionNamespace = actionNamespace;
            ActionName = actionName;
        }

        public RouteData RouteData { get; private set; }
        public string ActionNamespace { get; private set; }
        public string ActionName { get; private set; }
    }
}