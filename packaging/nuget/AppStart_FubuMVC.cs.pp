using System.Web.Routing;
using FubuMVC.Core;
using FubuMVC.StructureMap;
using StructureMap;

// You can remove the reference to WebActivator by calling the Start() method from your Global.asax Application_Start
[assembly: WebActivator.PreApplicationStartMethod(typeof($rootnamespace$.AppStart_FubuMVC), "Start", callAfterGlobalAppStart: true)]

namespace $rootnamespace$
{
    public static class AppStart_FubuMVC
    {
        public static void Start()
        {
            FubuApplication.For<ConfigureFubuMVC>()
                .StructureMap(() => new Container())
                .Bootstrap(RouteTable.Routes);
        }
    }
}