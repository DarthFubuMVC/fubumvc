using System;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.ServiceBus.Polling;
using FubuTestingSupport;
using NUnit.Framework;
using System.Linq;

namespace FubuTransportation.Testing.Polling
{
    [TestFixture]
    public class PollingServicesRegistry_spec
    {
        private void registeredTypeIs<TService, TImplementation>()
        {
            registeredTypeIs(typeof(TService), typeof(TImplementation));
        }

        private void registeredTypeIs(Type service, Type implementation)
        {
            services().DefaultServiceFor(service)
                .Type.ShouldEqual(implementation);
        }

        private static ServiceGraph services()
        {
            var registry = new FubuRegistry();
            registry.Services<PollingServicesRegistry>();
            var services = BehaviorGraph.BuildFrom(registry).Services;
            return services;
        }

        [Test]
        public void default_timer_is_the_timer()
        {
            registeredTypeIs<ITimer, DefaultTimer>();
        }

        [Test]
        public void polling_job_activator_is_registered()
        {
            services().ServicesFor<IActivator>()
                      .Any(x => x.Type == typeof (PollingJobActivator));
        }

        [Test]
        public void latch_is_registered()
        {
            services().DefaultServiceFor<PollingJobLatch>().IsSingleton
                .ShouldBeTrue();
        }

        [Test]
        public void polling_job_deactivator_is_registered()
        {
            services().ServicesFor<IDeactivator>()
                      .Any(x => x.Type == typeof(PollingJobDeactivator));
        }

        [Test]
        public void polling_jobs_is_registered()
        {
            registeredTypeIs<IPollingJobs, PollingJobs>();
            services().DefaultServiceFor<IPollingJobs>().IsSingleton
                .ShouldBeTrue();
        }

        [Test]
        public void polling_logger_is_registered()
        {
            registeredTypeIs<IPollingJobLogger, PollingJobLogger>();
        }
    }
}