using System;
using FubuMVC.Core;
using Microsoft.Practices.ServiceLocation;
using StructureMap;

namespace FubuMVC.StructureMap
{
    public static class BootstrappingExtensions
    {
        public static FubuApplication StructureMapObjectFactory(this IContainerFacilityExpression expression)
        {
            return expression.StructureMap(() => ObjectFactory.Container);
        } 

        public static FubuApplication StructureMapObjectFactory(this IContainerFacilityExpression expression, Action<IInitializationExpression> structureMapBootstrapper)
        {
            ObjectFactory.Initialize(structureMapBootstrapper);
            return expression.StructureMapObjectFactory();
        }

        public static FubuApplication StructureMap(this IContainerFacilityExpression expression, Func<IContainer> createContainer)
        {
            return expression.ContainerFacility(() =>
            {
                var container = createContainer();

                // TODO -- why is this necessary at all?
                ServiceLocator.SetLocatorProvider(() => new StructureMapServiceLocator(ObjectFactory.Container));

                return new StructureMapContainerFacility(container);
            });
        }
    }
}