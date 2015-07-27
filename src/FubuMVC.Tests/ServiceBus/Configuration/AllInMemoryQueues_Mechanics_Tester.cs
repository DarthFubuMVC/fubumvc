using System;
using System.Collections.Generic;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using NUnit.Framework;
using Shouldly;
using StructureMap;

namespace FubuMVC.Tests.ServiceBus.Configuration
{
    [TestFixture]
    public class AllInMemoryQueues_Mechanics_Tester
    {
        private IContainer container;
        private FubuRuntime runtime;

        [SetUp]
        public void SetUp()
        {
            FubuTransport.AllQueuesInMemory = true;

            var registry = new FubuRegistry();
            registry.AlterSettings<TransportSettings>(x =>
            {
                x.SettingTypes.Fill(typeof (BusSettings));
                x.SettingTypes.Fill(typeof (AnotherSettings));
            });

            registry.Features.ServiceBus.Enable(true);

            runtime = FubuApplication.For(registry).Bootstrap();
            container = runtime.Factory.Get<IContainer>();

        }

        [TearDown]
        public void TearDown()
        {
            runtime.Dispose();
        }

        [Test]
        public void we_remember_all_of_the_setting_types()
        {
            var transportSettings = container.GetInstance<TransportSettings>();
            transportSettings.SettingTypes.ShouldContain(typeof(BusSettings));
            transportSettings.SettingTypes.ShouldContain(typeof(AnotherSettings));
        }

        [Test]
        public void if_in_memory_queues_for_all_derive_the_queue_uri_and_have_it_injected_into_container()
        {

            var busSettings = container.GetInstance<BusSettings>();
            busSettings.Downstream.ToString().ShouldBe("memory://bus/downstream");

            var anotherSettings = container.GetInstance<AnotherSettings>();
            anotherSettings.Destination.ToString().ShouldBe("memory://another/destination");

        }
    }

    public class AllInMemoryRegistry : FubuTransportRegistry<BusSettings>
    {
        
    }

    public class AnotherRegistry : FubuTransportRegistry<AnotherSettings>
    {
        
    }

    public class AnotherSettings
    {
        public Uri Destination { get; set; }
    }
}