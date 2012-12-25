using System;

using Autofac;

using FubuMVC.Core;


namespace FubuMVC.Autofac
{
    public static class BootstrapperExtensions
    {
        public static FubuApplication Autofac(this IContainerFacilityExpression expression)
        {
            return expression.Autofac(
                () =>
                {
                    var builder = new ContainerBuilder();
                    return builder.Build();
                });
        }

        public static FubuApplication Autofac(this IContainerFacilityExpression expression, IComponentContext context)
        {
            return expression.Autofac(() => context);
        }

        public static FubuApplication Autofac(this IContainerFacilityExpression expression, Func<IComponentContext> createContext)
        {
            return expression.ContainerFacility(
                () =>
                {
                    IComponentContext context = createContext();
                    return new AutofacContainerFacility(context);
                });
        }
    }
}