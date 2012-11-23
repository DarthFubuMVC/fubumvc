using FubuMVC.Core.Bootstrapping;
using StructureMap;

namespace FubuMVC.StructureMap.Testing
{
    public static class ContainerFacilitySource
    {
         public static IContainerFacility New()
         {
             return new StructureMapContainerFacility(new Container());
         }
    }
}