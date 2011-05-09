using System.Web.Routing;
using FubuMVC.Core;
using FubuMVC.StructureMap;
using StructureMap;

// You can remove the reference to WebActivator by calling the Start() method from your Global.asax Application_Start
[assembly: WebActivator.PreApplicationStartMethod(typeof($rootnamespace$.App_Start.AppStartFubuMVC), "Start", callAfterGlobalAppStart: true)]

namespace $rootnamespace$.App_Start
{
    public static class AppStartFubuMVC
    {
        public static void Start()
        {
	    // ConfigureFubuMVC is the top level FubuRegistry
	    // for this application
            FubuApplication.For<ConfigureFubuMVC>()
                .StructureMap(() => new Container())
                .Bootstrap(RouteTable.Routes);
        }
    }
}