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

    public static class ContainerExtensions
    {
        public static void InitializeFubuApplication(this IContainer container, FubuRegistry fubuRegistry, ICollection<RouteBase> routes)
        {
            var bootstrapper = new StructureMapBootstrapper(container, fubuRegistry);
            bootstrapper.Bootstrap(routes);
        }
    }

    public class StructureMapBootstrapper : FubuBootstrapper
    {
        private readonly StructureMapContainerFacility _smFacility;

        public StructureMapBootstrapper(IContainer container, FubuRegistry topRegistry)
            : this(new StructureMapContainerFacility(container), topRegistry)
        {
        }

        public StructureMapBootstrapper(StructureMapContainerFacility facility, FubuRegistry registry)
            : base(facility, registry)
        {
            _smFacility = facility;
        }

        public Func<IContainer, ServiceArguments, Guid, IActionBehavior> Builder { get { return _smFacility.Builder; } set { _smFacility.Builder = value; } }
    
    
        public static IContainer BuildContainer(FubuRegistry registry)
        {
            var container = new Container();
            var bootstrapper = new StructureMapBootstrapper(container, registry);
            bootstrapper.Bootstrap(new List<RouteBase>());

            return container;
        }
    }
}