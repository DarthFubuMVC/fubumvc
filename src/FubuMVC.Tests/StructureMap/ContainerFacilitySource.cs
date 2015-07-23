using System;
using FubuCore;
using FubuMVC.Core.Bootstrapping;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.StructureMap;
using StructureMap;

namespace FubuMVC.Tests.StructureMap
{
    public static class ContainerFacilitySource
    {
        public static IServiceFactory New(Action<IContainerFacility> configure)
        {
            var facility = new StructureMapContainerFacility(new Container());
            configure(facility);

            // A ContainerFacility cannot be considered "ready" for business until BuildFactory() has been
            // called
            return facility.BuildFactory(new BehaviorGraph());
        }

        public static IServiceLocator Services(Action<IContainerFacility> configure)
        {
            var facility = new StructureMapContainerFacility(new Container());
            configure(facility);

            // A ContainerFacility cannot be considered "ready" for business until BuildFactory() has been
            // called
            return facility.BuildFactory(new BehaviorGraph()).Get<IServiceLocator>();
        }
    }
}