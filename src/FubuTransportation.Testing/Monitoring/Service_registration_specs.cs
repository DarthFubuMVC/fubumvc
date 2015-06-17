using System;
using System.Linq;
using FubuCore.Logging;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuTestingSupport;
using FubuTransportation.Monitoring;
using NUnit.Framework;

namespace FubuTransportation.Testing.Monitoring
{
    [TestFixture]
    public class Service_registration_specs
    {
        private ServiceGraph services;

        [TestFixtureSetUp]
        public void SetUp()
        {
            FubuMode.RemoveTestingMode();
            var registry = new FubuRegistry();
            registry.Services<MonitoringServiceRegistry>();
            services = BehaviorGraph.BuildFrom(registry)
                .Services;
        }

        private void registeredTypeIs<TService, TImplementation>()
        {
            registeredTypeIs(typeof(TService), typeof(TImplementation));
        }

        private void registeredTypeIs(Type service, Type implementation)
        {
            services.DefaultServiceFor(service)
                .Type.ShouldEqual(implementation);
        }

        [Test]
        public void log_modifier_for_persistent_task_messages_is_registered()
        {
            services.ServicesFor<ILogModifier>().Any(x => x.Type == typeof(PersistentTaskMessageModifier))
                .ShouldBeTrue();
        }

        [Test]
        public void controller_is_registered()
        {
            registeredTypeIs<IPersistentTaskController, PersistentTaskController>();
        }

        [Test]
        public void transport_peer_repository()
        {
            registeredTypeIs<ITaskMonitoringSource, TaskMonitoringSource>();
        }
    }
}