using System;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.InMemory;
using FubuMVC.Core.ServiceBus.Scheduling;
using Xunit;
using Shouldly;
using StructureMap;

namespace FubuMVC.Tests.ServiceBus.Configuration
{
    
    public class ChannelGraph_can_read_thread_and_task_counts_from_config : IDisposable
    {
        private Container theContainer;
        private FubuRuntime theRuntime;
        private ChannelGraph theGraph;
        private ConfiguredSettings theSettings;

        public ChannelGraph_can_read_thread_and_task_counts_from_config()
        {
            InMemoryQueueManager.ClearAll();

            theSettings = new ConfiguredSettings
            {
                Upstream = "memory://foo".ToUri(),
                Outbound = "memory://bar".ToUri()
            };

            theContainer = new Container(x => {
                x.For<ConfiguredSettings>().Use(theSettings);
            });

            var registry = new ConfiguredFubuRegistry();
            registry.StructureMap(theContainer);

            theRuntime = registry.ToRuntime();

            theGraph = theContainer.GetInstance<ChannelGraph>();
        }

        public void Dispose()
        {
            theRuntime.Dispose();
        }


        [Fact]
        public void can_read_settings_to_create_task_schedulers()
        {
            theContainer.GetInstance<ConfiguredSettings>()
                .ShouldBeTheSameAs(theSettings);

            theContainer.GetInstance<ConfiguredSettings>()
                .OutboundCount.ShouldBe(5);

            theContainer.GetInstance<ConfiguredSettings>()
                .UpstreamCount.ShouldBe(7);

            theGraph.ChannelFor<ConfiguredSettings>(x => x.Outbound)
                .Scheduler
                .ShouldBeOfType<TaskScheduler>()
                .TaskCount.ShouldBe(theSettings.OutboundCount);
        }


        [Fact]
        public void can_read_settings_to_create_thread_schedulers()
        {
            theGraph.ChannelFor<ConfiguredSettings>(x => x.Upstream)
                .Scheduler
                .ShouldBeOfType<ThreadScheduler>()
                .ThreadCount.ShouldBe(theSettings.UpstreamCount);
        }

        public class ConfiguredFubuRegistry : FubuTransportRegistry<ConfiguredSettings>
        {
            public ConfiguredFubuRegistry()
            {
                ServiceBus.EnableInMemoryTransport();

                Channel(x => x.Outbound).ReadIncoming(ByTasks(x => x.OutboundCount));
                Channel(x => x.Upstream).ReadIncoming(ByThreads(x => x.UpstreamCount));
            }
        }

        public class ConfiguredSettings
        {

            public ConfiguredSettings()
            {
                UpstreamCount = 7;
                OutboundCount = 5;

            }

            public Uri Outbound { get; set; }
            public Uri Downstream { get; set; }
            public Uri Upstream { get; set; }

            public int UpstreamCount { get; private set; }
            public int OutboundCount { get; private set; }
        }
    }
}