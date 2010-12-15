using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Routing;
using FubuMVC.Core;
using FubuMVC.Core.Packaging;
using StructureMap;

namespace FubuMVC.StructureMap.Bootstrap
{
    public class FubuStructureMapApplication : HttpApplication
    {
        private string _controllerAssembly;
        private bool? _enableDiagnostics;

        public bool EnableDiagnostics
        {
            get { return _enableDiagnostics ?? HttpContext.Current.IsDebuggingEnabled; }
            set { _enableDiagnostics = value; }
        }

        public string ControllerAssembly
        {
            get { return _controllerAssembly ?? FindClientCodeAssembly(GetType().Assembly); }
            set { _controllerAssembly = value; }
        }

        private static string FindClientCodeAssembly(Assembly globalAssembly)
        {
            return globalAssembly
                .GetReferencedAssemblies()
                .First(name => !(name.Name.Contains("System.") && !(name.Name.Contains("mscorlib"))))
                .Name;
        }

        public virtual FubuRegistry GetMyRegistry()
        {
            return new BasicFubuStructureMapRegistry(HttpContext.Current.IsDebuggingEnabled, ControllerAssembly);
        }

        // TODO -- think this needs to go away
        protected virtual void InitializeValidation()
        {
        }

        // Rather have this replaced with something that returns an IContainer
        protected virtual void InitializeStructureMap(IInitializationExpression ex)
        {
            // no op, please override
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            var routeCollection = RouteTable.Routes;

            Bootstrap(routeCollection);
        }

        [SkipOverForProvenance]
        public void Bootstrap(ICollection<RouteBase> routes)
        {
            FubuApplication.For(GetMyRegistry())
                .StructureMapObjectFactory(InitializeStructureMap)
                .Bootstrap(routes);
        }
    }
}