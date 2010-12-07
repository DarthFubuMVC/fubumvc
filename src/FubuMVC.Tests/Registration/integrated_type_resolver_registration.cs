using System;
using FubuCore;
using FubuMVC.Core;
using NUnit.Framework;

namespace FubuMVC.Tests.Registration
{
    [TestFixture]
    public class integrated_type_resolver_registration
    {
        [Test]
        public void can_register_new_type_resolution_strategies()
        {
            var registry = new FubuRegistry();
            registry.ResolveTypes(x => x.AddStrategy<ProxyDetector>());

            var graph = registry.BuildGraph();

            var resolver = graph.Services.DefaultServiceFor<ITypeResolver>().Value.ShouldBeOfType<TypeResolver>();

            resolver.ResolveType(new Proxy<UniqueInput>()).ShouldEqual(typeof (UniqueInput));
        }
    }

    public class UniqueInput { }

    public class ProxyDetector : ITypeResolverStrategy
    {
        public Type ResolveType(object model)
        {
            return model.GetType().GetGenericArguments()[0];
        }

        public bool Matches(object model)
        {
            return model.GetType().Closes(typeof(Proxy<>));
        }
    }

    public class Proxy<T> { }
}