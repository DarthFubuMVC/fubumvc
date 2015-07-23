using System;
using System.Linq;
using Shouldly;
using StructureMap;
using StructureMap.Pipeline;

namespace FubuMVC.Tests
{
    public static class ContainerExtensions
    {
        public static IContainer DefaultRegistrationIs<T, TConcrete>(this IContainer container) where TConcrete : T
        {
            container.Model.DefaultTypeFor<T>().ShouldBe(typeof (TConcrete));
            return container;
        }

        public static IContainer DefaultRegistrationIs(this IContainer container, Type pluginType, Type concreteType)
        {
            container.Model.DefaultTypeFor(pluginType).ShouldBe(concreteType);

            return container;
        }

        public static IContainer DefaultRegistrationIs<T>(this IContainer container, T value) where T : class
        {
            container.Model.For<T>().Default.Get<T>().ShouldBeTheSameAs(value);

            return container;
        }

        public static IContainer DefaultSingletonIs(this IContainer container, Type pluginType, Type concreteType)
        {
            container.DefaultRegistrationIs(pluginType, concreteType);
            container.Model.For(pluginType).Default.Lifecycle.ShouldBeOfType<SingletonLifecycle>();

            return container;
        }

        public static IContainer DefaultSingletonIs<T, TConcrete>(this IContainer container) where TConcrete : T
        {
            container.DefaultRegistrationIs<T, TConcrete>();
            container.Model.For<T>().Default.Lifecycle.ShouldBeOfType<SingletonLifecycle>();

            return container;
        }

        public static IContainer ShouldHaveRegistration<T, TConcrete>(this IContainer container)
        {
            var plugin = container.Model.For<T>();
            plugin.Instances.Any(x => x.ReturnedType == typeof (TConcrete)).ShouldBeTrue();

            return container;
        }

        public static IContainer ShouldNotHaveRegistration<T, TConcrete>(this IContainer container)
        {
            var plugin = container.Model.For<T>();
            plugin.Instances.Any(x => x.ReturnedType == typeof(TConcrete)).ShouldBeFalse();

            return container;
        }
    }
}