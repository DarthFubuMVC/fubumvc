using System;
using System.Collections.Generic;
using System.Web.Routing;
using FubuCore.Binding;
using FubuMVC.Core;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;
using StructureMap;

namespace FubuMVC.StructureMap
{
    [Obsolete("Please use FubuApplication instead")]
    public static class ContainerExtensions
    {
        public static void InitializeFubuApplication(this IContainer container, FubuRegistry fubuRegistry, ICollection<RouteBase> routes)
        {
            var bootstrapper = new StructureMapBootstrapper(container, fubuRegistry);
            bootstrapper.Bootstrap(routes);
        }
    }

    [Obsolete("This class is going to be removed in the near future (at least before an official release).  Please use FubuApplication instead")]
    public class StructureMapBootstrapper : FubuBootstrapper
    {
        public StructureMapBootstrapper(IContainer container, FubuRegistry topRegistry)
            : base(new StructureMapContainerFacility(container), topRegistry)
        {
        }

        public static IContainer BuildContainer(FubuRegistry registry)
        {
            var container = new Container();
            var bootstrapper = new StructureMapBootstrapper(container, registry);
            bootstrapper.Bootstrap(new List<RouteBase>());

            return container;
        }
    }
}