// SAMPLE: asp-net-hello-world-application
using System;
using FubuMVC.Core;
using FubuMVC.StructureMap;

namespace QuickStart
{
    public class Global : System.Web.HttpApplication
    {
        private FubuRuntime _runtime;

        protected void Application_Start(object sender, EventArgs e)
        {
            _runtime = FubuApplication
                .DefaultPolicies()  // Use the default FubuMVC conventions
                .StructureMap()     // Use a new StructureMap container
                .Bootstrap();    
        }

        protected void Application_End(object sender, EventArgs e)
        {
            _runtime.Dispose();
        }
    }
}
// ENDSAMPLE