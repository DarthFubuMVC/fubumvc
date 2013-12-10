using Castle.Windsor;
using FubuMVC.Core;

namespace FubuMVC.Windsor
{
    public static class BootstrappingExtensions
    {
        public static FubuApplication Windsor(this IContainerFacilityExpression expression, IWindsorContainer container)
        {
            return expression.ContainerFacility(new WindsorContainerFacility(container));
        }

        public static FubuApplication Windsor(this IContainerFacilityExpression expression)
        {
            return expression.ContainerFacility(new WindsorContainerFacility(new WindsorContainer()));
        }
    }
}