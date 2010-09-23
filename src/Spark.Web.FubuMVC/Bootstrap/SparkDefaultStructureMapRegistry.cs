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
            
            Func<Type, bool> actionNameFilter = actionType => actionType.Name.EndsWith("Controller");
            Func<string, string> actionNameConvention = action => action.RemoveSuffix("Controller");

            Views.Facility(new SparkViewFacility(viewFactory, actionNameFilter, actionNameConvention))
                .TryToAttach(x => x.BySparkViewDescriptors(actionNameConvention));
        }
    }
}