using System.Web;
using System.Web.Routing;

namespace FubuMVC.HelloWorld
{
    public class Global : HttpApplication
    {
        protected void Application_Start()
        {
            FubuStructureMapBootstrapper.Bootstrap(RouteTable.Routes);
        }
    }
}