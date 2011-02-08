using System;
using System.Web;
using System.Web.Routing;
using FubuMVC.Core;
using FubuMVC.Core.Packaging;
using FubuMVC.StructureMap;

namespace FubuTestApplication
{
    public class Global : HttpApplication
    {
		protected void Application_Start(object sender, EventArgs e)
		{
			FubuApplication
				.For<FubuTestApplicationRegistry>()
				.StructureMapObjectFactory()
				.Bootstrap(RouteTable.Routes);

			PackageRegistry.AssertNoFailures();
		}
    }

    public class FubuTestApplicationRegistry : FubuRegistry
    {
        public FubuTestApplicationRegistry()
        {
            IncludeDiagnostics(true);

            Actions.IncludeType<ScriptsHandler>();
        }
    }
}