using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;

namespace FubuMVC.IntegrationTesting.ServiceBus
{
    public class PerformanceTester
    {
        public static int Times = 2000;

        public static void send_and_respond_roundtrip()
        {
            var waitHandle = new ManualResetEvent(false);
            send_and_respond_roundtrip_helper()
                .ContinueWith(x => waitHandle.Set());
            waitHandle.WaitOne();
        }

        private static async Task send_and_respond_roundtrip_helper()
        {
            using (var runtime = FubuRuntime.For<PingerTransportRegistry>())
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
                        bus.Send(new PingMessage());
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
    }


    public class PingerTransportRegistry : FubuTransportRegistry<PingerSettings>
    {
        public PingerTransportRegistry()
        {
            Channel(x => x.Pinger)
                .AcceptsMessage<PingMessage>()
                .AcceptsMessage<PongMessage>()
                .ReadIncoming();
        }
    }

    public class PingerSettings
    {
        public PingerSettings()
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
        public static TaskCompletionSource<bool> ReceivedAll = new TaskCompletionSource<bool>();
        public static int Count;

        public void Consume(PongMessage message)
        {
            var times = Interlocked.Increment(ref Count);
            if(times == PerformanceTester.Times)
                ReceivedAll.SetResult(true);
        }
    }
}