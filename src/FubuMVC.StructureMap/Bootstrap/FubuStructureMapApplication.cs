using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Routing;
using FubuCore;
using FubuCore.Binding;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
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

        protected virtual void InitializeValidation()
        {
        }

        protected virtual void InitializeStructureMap(IInitializationExpression ex)
        {
            // no op, please override
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            var routeCollection = RouteTable.Routes;

            Bootstrap(routeCollection);
        }

        public void Bootstrap(ICollection<RouteBase> routes)
        {
            PackageRegistry.LoadPackages(x =>
            {
                x.Facility(new FubuMvcPackageFacility());
                x.Bootstrap(log =>
                {
                    var fubuRegistry = GetMyRegistry();

                    BootstrapStructureMap(routes, fubuRegistry, InitializeStructureMap);
                    return ObjectFactory.GetAllInstances<IPackageActivator>();
                });
            });
        }

        private void BootstrapStructureMap(ICollection<RouteBase> routes, FubuRegistry fubuRegistry,
                                           Action<IInitializationExpression> initializeExpression)
        {
            UrlContext.Reset();

            InitializeValidation();

            ObjectFactory.Initialize(initializeExpression);

            var fubuBootstrapper = new StructureMapBootstrapper(ObjectFactory.Container, fubuRegistry);

            fubuBootstrapper.Bootstrap(routes);

            var existingBuilder = fubuBootstrapper.Builder;

            fubuBootstrapper.Builder = ((container, args, id) =>
                                        GetBuilder(container, args, id) ?? existingBuilder(container, args, id));
        }

        protected virtual IActionBehavior GetBuilder(IContainer container, ServiceArguments args, Guid beehaviorId)
        {
            return null;
        }
    }
}