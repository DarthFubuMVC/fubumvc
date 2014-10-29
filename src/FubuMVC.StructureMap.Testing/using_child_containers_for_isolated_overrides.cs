using System;
using FubuCore;
using FubuMVC.Core;
using FubuTestingSupport;
using NUnit.Framework;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace FubuMVC.StructureMap.Testing
{
    [TestFixture]
    public class using_child_containers_for_isolated_overrides
    {
        [Test]
        public void override_services()
        {
            using (var runtime = FubuApplication.DefaultPolicies().StructureMap<ThingRegistry>().Bootstrap())
            {
                var facility = runtime.Facility.As<StructureMapContainerFacility>();

                facility.Get<ThingUser>().Thing.ShouldBeOfType<AThing>();

                facility.StartNewScope();

                facility.Container.Role.ShouldEqual(ContainerRole.ProfileOrChild);

                facility.Container.Configure(_ => _.For<IThing>().Use<BThing>().Singleton());

                var bthing = facility.Get<ThingUser>().Thing.ShouldBeOfType<BThing>();

                facility.TeardownScope();

                // back to normal
                facility.Get<ThingUser>().Thing.ShouldBeOfType<AThing>();
            }
        }
    }

    public class ThingRegistry : Registry
    {
        public ThingRegistry()
        {
            For<IThing>().Use<AThing>().Singleton();
        }
    }

    public class ThingUser
    {
        public IThing Thing { get; set; }

        public ThingUser(IThing thing)
        {
            Thing = thing;
        }
    }

    public interface IThing
    {
        
    }

    public class AThing : IThing, IDisposable
    {
        public void Dispose()
        {
            WasDisposed = true;
        }

        public bool WasDisposed { get; set; }
    }

    public class BThing : AThing
    {
        
    }
}