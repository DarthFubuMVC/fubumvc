using System;
using StructureMap;
using FubuMVC.Core;
using Spark.Web.FubuMVC.Extensions;
using Spark.Web.FubuMVC.ViewLocation;

namespace Spark.Web.FubuMVC.Bootstrap
{
    public class SparkDefaultStructureMapRegistry : FubuRegistry
    {
        public SparkDefaultStructureMapRegistry(bool debuggingEnabled, string controllerAssembly)
        {
            var viewFactory = ObjectFactory.Container.GetInstance<SparkViewFactory>();

            IncludeDiagnostics(debuggingEnabled);

            Applies.ToAssembly(controllerAssembly);

            Actions.IncludeTypesNamed(x => x.EndsWith("Controller"));

            Routes.IgnoreControllerNamespaceEntirely();

            Views.Facility(new SparkViewFacility(viewFactory, actionType => actionType.Name.EndsWith("Controller")))
                .TryToAttach(x => x.BySparkViewDescriptors(action => action.RemoveSuffix("Controller")));
        }
    }
}