using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.InMemory;
using FubuMVC.Core.ServiceBus.Runtime;
using NUnit.Framework;

namespace FubuMVC.Tests.ServiceBus.InMemory
{
    [TestFixture]
    public class InMemoryTransport_opt_in_registration_tester
    {
        [Test]
        public void is_not_registered_normally()
        {
            FubuTransport.Reset();
            

            using (var runtime = FubuRuntime.Basic())
            {
                runtime.Container.ShouldNotHaveRegistration<ITransport, InMemoryTransport>();
            }
        }

        [Test]
        public void is_registered_if_FubuTransport_in_memory_testing()
        {
            FubuTransport.AllQueuesInMemory = true;

            using (var runtime = FubuTransport.DefaultPolicies().Bootstrap())
            {
                runtime.Container.ShouldHaveRegistration<ITransport, InMemoryTransport>();
            }
        }

        [Test]
        public void is_registered_if_user_opted_into_in_memory_transport()
        {
            FubuTransport.Reset();

            var registry = new FubuRegistry();
            registry.ServiceBus.Configure(_ =>
            {
                _.EnableInMemoryTransport = true;
                _.Enabled = true;
            });

            using (var runtime = FubuTransport.DefaultPolicies().Bootstrap())
            {
                runtime.Container.ShouldHaveRegistration<ITransport, InMemoryTransport>();
            }
        }
    }
}