using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Routing;
using FubuMVC.Core;

namespace Spark.Web.FubuMVC.Bootstrap
{
    public class SparkStructureMapApplication : HttpApplication
    {
        private string _controllerAssembly;
        private bool? _enableDiagnostics;
        private SparkViewFactory _viewFactory;

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

        public SparkViewFactory ViewFactory
        {
            get { return _viewFactory ?? CreateViewFactory(); }
            set { _viewFactory = value; }
        }

        private static SparkViewFactory CreateViewFactory()
        {
            //This is the default, but don't forget, you can customize a ton of Spark settings here.
            return new SparkViewFactory(new SparkSettings());
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
            return new SparkDefaultStructureMapRegistry(EnableDiagnostics, ControllerAssembly, ViewFactory);
        }


        protected void Application_Start(object sender, EventArgs e)
        {
            RouteCollection routeCollection = RouteTable.Routes;
            SparkStructureMapBootstrapper.Bootstrap(routeCollection, GetMyRegistry());
            OnApplicationStarted();
        }

        protected virtual void OnApplicationStarted()
        {
            
        }
    }
}