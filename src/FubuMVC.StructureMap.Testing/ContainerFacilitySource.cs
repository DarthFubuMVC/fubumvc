using System;
using FubuMVC.Core.Bootstrapping;
using FubuMVC.Core.Runtime;
using StructureMap;

namespace FubuMVC.StructureMap.Testing
{
    public static class ContainerFacilitySource
    {
         public static IServiceFactory New(Action<IContainerFacility> configure)
         {
             var facility = new StructureMapContainerFacility(new Container());
             configure(facility);

             // A ContainerFacility cannot be considered "ready" for business until BuildFactory() has been
             // called
             return facility.BuildFactory();
         }
    }
}