using System.Web;
using System.Web.Routing;
using Bottles;

using FubuMVC.Core;
using FubuMVC.StructureMap;
using StructureMap;
using FubuMVC.Core.UI;


namespace FubuTestApplication
{
    public class Global : HttpApplication
    {
        protected void Application_Start()
        {
            // TODO -- add smart grid controllers
            FubuApplication.For<FubuTestApplicationRegistry>()
                .StructureMap(new Container())
                .Bootstrap();

            PackageRegistry.AssertNoFailures();
        }
    }


}