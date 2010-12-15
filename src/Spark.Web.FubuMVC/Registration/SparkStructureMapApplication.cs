
using FubuMVC.Core.UI.Tags;
using FubuMVC.StructureMap;
using Microsoft.Practices.ServiceLocation;
using Spark.Web.FubuMVC.ViewCreation;
using StructureMap;
using System;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Routing;

namespace Spark.Web.FubuMVC.Registration
{
    public class SparkStructureMapApplication : HttpApplication
    {
        private string _controllerAssembly;
        private bool? _enableDiagnostics;

        public bool EnableDiagnostics { get { return _enableDiagnostics ?? HttpContext.Current.IsDebuggingEnabled; } set { _enableDiagnostics = value; } }

        public string ControllerAssembly { get { return _controllerAssembly ?? FindClientCodeAssembly(GetType().Assembly); } set { _controllerAssembly = value; } }

        private static string FindClientCodeAssembly(Assembly globalAssembly)
        {
            return globalAssembly
                .GetReferencedAssemblies()
                .First(name => !(name.Name.Contains("System.") && !(name.Name.Contains("mscorlib"))))
                .Name;
        }

        protected virtual SparkSettings GetSparkSettings()
        {
            return new SparkSettings()
                .AddAssembly(typeof(PartialTagFactory).Assembly)
                .AddNamespace("Spark.Web.FubuMVC")
                .AddNamespace("FubuMVC.Core.UI")
                .AddNamespace("HtmlTags");
        }

        protected virtual void InitializeStructureMap(IInitializationExpression ex)
        {
            ex.ForSingletonOf<SparkViewFactory>();
            ex.For<IServiceLocator>().Use<StructureMapServiceLocator>();
            ex.For<ISparkSettings>().Use(GetSparkSettings);
            ex.For(typeof(ISparkViewRenderer<>)).Use(typeof(SparkViewRenderer<>));
        }

        public virtual SparkFubuRegistry GetMyRegistry()
        {
            return ObjectFactory.GetInstance<SparkFubuRegistry>();
        }


        protected void Application_Start(object sender, EventArgs e)
        {
            var routeCollection = RouteTable.Routes;
            ObjectFactory.Initialize(InitializeStructureMap);
            SparkStructureMapBootstrapper.Bootstrap(routeCollection, GetMyRegistry());
            OnApplicationStarted();
        }

        protected virtual void OnApplicationStarted()
        {
            
        }
    }

}