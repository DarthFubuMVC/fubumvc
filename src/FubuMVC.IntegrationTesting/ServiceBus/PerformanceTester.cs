using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Polling;

namespace FubuMVC.IntegrationTesting.ServiceBus
{
    public class PerformanceTester
    {
        public static int Times = 1000;

        public static void send_and_respond_roundtrip()
        {
            var waitHandle = new ManualResetEvent(false);
            send_and_respond_roundtrip_helper()
                .ContinueWith(x => waitHandle.Set());
            waitHandle.WaitOne();
        }

        private static async Task send_and_respond_roundtrip_helper()
        {
            await perform_work<SingleEndpointTransportRegistry>(x => x.Send(new PingMessage()));
        }

        private static async Task perform_work<TRegistry>(Action<IServiceBus> actualWork) where TRegistry : FubuRegistry, new()
        {
            PongConsumer.Count = 0;
            PongConsumer.ReceivedAll = new TaskCompletionSource<bool>();
            using (var runtime = FubuRuntime.For<TRegistry>())
            {
                var bus = runtime.Get<IServiceBus>();
                var tasks = new List<Task>();
                for (var i = 0; i < Times; ++i)
                {
                    if (i == 10)
                    {
                        if (JetBrains.Profiler.Windows.Api.PerformanceProfiler.IsActive)
                        {
                            JetBrains.Profiler.Windows.Api.PerformanceProfiler.Begin();
                            JetBrains.Profiler.Windows.Api.PerformanceProfiler.Start();
                        }
                    }
                    var task = Task.Run(() =>
                    {
                        actualWork(bus);
                    });
                    tasks.Add(task);
                }
                await Task.WhenAll(tasks);
                await PongConsumer.ReceivedAll.Task;
                if (JetBrains.Profiler.Windows.Api.PerformanceProfiler.IsActive)
                {
                    JetBrains.Profiler.Windows.Api.PerformanceProfiler.Stop();
                    JetBrains.Profiler.Windows.Api.PerformanceProfiler.EndSave();
                }
            }
        }

        public static void send_and_respond_roundtrip_separate_endpoints()
        {
            var waitHandle = new ManualResetEvent(false);
            send_and_respond_roundtrip_separate_endpoints_helper()
                .ContinueWith(x => waitHandle.Set());
            waitHandle.WaitOne();
        }

        private static async Task send_and_respond_roundtrip_separate_endpoints_helper()
        {
            using (FubuRuntime.For<PongerTransportRegistry>())
            {
                await perform_work<PingerTransportRegistry>(x => x.Send(new PingMessage()));
            }
        }
    }

    public class PingerTransportRegistry : FubuTransportRegistry<PingPongSettings>
    {
        public PingerTransportRegistry()
        {
            ServiceBus.HealthMonitoring.ScheduledExecution(ScheduledExecution.Disabled);
            Channel(x => x.Pinger)
                .AcceptsMessage<PongMessage>()
                .ReadIncoming();

            Handlers.DisableDefaultHandlerSource();
            Handlers.Include<PongConsumer>();
            Channel(x => x.Ponger)
                .AcceptsMessage<PingMessage>();
        }
    }

    public class PongerTransportRegistry : FubuTransportRegistry<PingPongSettings>
    {
        public PongerTransportRegistry()
        {
            ServiceBus.HealthMonitoring.ScheduledExecution(ScheduledExecution.Disabled);
            Channel(x => x.Ponger)
                .AcceptsMessage<PingMessage>()
                .ReadIncoming();

            Handlers.DisableDefaultHandlerSource();
            Handlers.Include<PingConsumer>();
            Channel(x => x.Pinger)
                .AcceptsMessage<PongMessage>();
        }
    }

    public class PingPongSettings
    {
        public PingPongSettings()
        {
            Pinger = new Uri("lq.tcp://localhost:2200/pinger");
            Ponger = new Uri("lq.tcp://localhost:2201/ponger");
        }

        public Uri Pinger { get; set; }
        public Uri Ponger { get; set; }
    }


    public class SingleEndpointTransportRegistry : FubuTransportRegistry<SingleEndpointSettings>
    {
        public SingleEndpointTransportRegistry()
        {
            ServiceBus.HealthMonitoring.ScheduledExecution(ScheduledExecution.Disabled);
            Handlers.DisableDefaultHandlerSource();
            Handlers.Include<PingConsumer>();
            Handlers.Include<PongConsumer>();
            Channel(x => x.Pinger)
                .AcceptsMessage<PingMessage>()
                .AcceptsMessage<PongMessage>()
                .ReadIncoming();
        }
    }

    public class SingleEndpointSettings
    {
        public SingleEndpointSettings()
        {
            Pinger = new Uri("lq.tcp://localhost:2200/pinger");
        }

        public Uri Pinger { get; set; }
    }

    public class PingMessage
    {
    }

    public class PongMessage
    {
    }

    public class PingConsumer
    {
        public PongMessage Consume(PingMessage message)
        {
            return new PongMessage();
        }
    }

    public class PongConsumer
    {
        public static TaskCompletionSource<bool> ReceivedAll;
        public static int Count;

        public void Consume(PongMessage message)
        {
            var times = Interlocked.Increment(ref Count);
            if(times == PerformanceTester.Times)
                ReceivedAll.SetResult(true);
        }
    }
}