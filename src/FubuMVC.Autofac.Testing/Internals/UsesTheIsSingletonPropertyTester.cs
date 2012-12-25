using System.Linq;

using Autofac;
using Autofac.Core;
using Autofac.Core.Lifetime;

using FubuCore.Dates;

using FubuMVC.Core;

using FubuTestingSupport;

using NUnit.Framework;


namespace FubuMVC.Autofac.Testing.Internals
{
    [TestFixture]
    public class UsesTheIsSingletonPropertyTester
    {
        [Test]
        public void IClock_should_be_a_singleton_just_by_usage_of_the_IsSingleton_property()
        {
            var builder = new ContainerBuilder();
            IContainer container = builder.Build();
            FubuApplication.For(new FubuRegistry()).Autofac(container).Bootstrap();

            var registration = container.ComponentRegistry.RegistrationsFor(new TypedService(typeof(IClock))).First();
            registration.Sharing.ShouldEqual(InstanceSharing.Shared);
            registration.Lifetime.GetType().ShouldEqual(typeof(RootScopeLifetime));
        }
    }
}