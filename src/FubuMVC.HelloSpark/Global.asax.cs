using System.Web;
using FubuMVC.Core;
using FubuMVC.StructureMap;
using System.Web.Routing;

namespace FubuMVC.HelloSpark
{
    public class Global : HttpApplication
    {
        protected void Application_Start()
        {
            FubuApplication.For(new HelloSparkRegistry())
                .StructureMapObjectFactory()
                .Bootstrap(RouteTable.Routes);
        }
    }
}