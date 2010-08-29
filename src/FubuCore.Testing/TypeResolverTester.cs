using System;
using NUnit.Framework;

namespace FubuCore.Testing
{
    [TestFixture]
    public class TypeResolverTester
    {
        [Test]
        public void basic_mode_just_returns_the_type_of_the_object()
        {
            new TypeResolver().ResolveType(new InputModel()).ShouldEqual(typeof (InputModel));
        }

        [Test]
        public void use_a_different_strategy_that_catches()
        {
            var resolver = new TypeResolver();
            resolver.AddStrategy<ProxyDetector>();

            resolver.ResolveType(new InputModel()).ShouldEqual(typeof (InputModel));
            resolver.ResolveType(new Proxy<InputModel>()).ShouldEqual(typeof (InputModel));
        }

        [Test]
        public void null_returns_null()
        {
            new TypeResolver().ResolveType(null).ShouldBeNull();
        }
    }

    public class InputModel
    {
        
    }

    public class ProxyDetector : ITypeResolverStrategy
    {
        public Type ResolveType(object model)
        {
            return model.GetType().GetGenericArguments()[0];
        }

        public bool Matches(object model)
        {
            return model.GetType().Closes(typeof (Proxy<>));
        }
    }

    public class Proxy<T>{}
}