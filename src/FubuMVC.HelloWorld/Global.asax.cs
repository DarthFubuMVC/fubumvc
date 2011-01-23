using System.Web;
using System.Web.Routing;
using FubuMVC.Core;
using FubuMVC.Core.Packaging;
using FubuMVC.HelloWorld.Services;
using StructureMap;
using FubuMVC.StructureMap;

namespace FubuMVC.HelloWorld
{
    public class Global : HttpApplication
    {
        protected void Application_Start()
        {
            FubuApplication
                .For<HelloWorldFubuRegistry>()
                .StructureMap(() =>
                {
                    return new Container(x => x.For<IHttpSession>().Use<CurrentHttpContextSession>());    
                })
                .Bootstrap(RouteTable.Routes);

            // If there is an error during bootstrapping, it will not automatically be considered
            // fatal and there will be no YSOD.  This is to help during initial debugging and 
            // troubleshooting package loading. Normally, however, you want a YSOD if there is
            // a bootstrapping failure or a package-loading failure. This next line ensures that.
            PackageRegistry.AssertNoFailures(); 
        }

    }
}