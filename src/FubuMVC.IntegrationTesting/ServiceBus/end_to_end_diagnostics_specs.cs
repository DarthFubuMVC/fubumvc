using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using FubuCore;
using FubuCore.Logging;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics.Instrumentation;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.LightningQueues;
using NUnit.Framework;
using Shouldly;
using TraceLevel = FubuMVC.Core.TraceLevel;

namespace FubuMVC.IntegrationTesting.ServiceBus
{
    [TestFixture]
    public class end_to_end_diagnostics_specs
    {
        [Test]
        public void see_tracing_logs_in_production_mode_happy_path()
        {
            var registry = new FubuRegistry();
            registry.ServiceBus.Enable(true);
            registry.Features.Diagnostics.Enable(TraceLevel.Production);
            registry.ServiceBus.EnableInMemoryTransport();
            registry.AlterSettings<LightningQueueSettings>(x => x.DisableIfNoChannels = true);

            using (var runtime = registry.ToRuntime())
            {
                var bus = runtime.Get<IServiceBus>();

                bus.Consume(new TracedInput());

                var history = runtime.Get<IChainExecutionHistory>();

                var log = history.RecentReports().Single(x => x.RootChain != null && x.RootChain.InputType() == typeof(TracedInput));

                log.Steps.Any(x => x.Log is StringMessage).ShouldBeFalse();

            }
        }

        [Test]
        public void get_error_messages_in_production_mode()
        {
            var registry = new FubuRegistry();
            registry.ServiceBus.Enable(true);
            registry.Features.Diagnostics.Enable(TraceLevel.Production);
            registry.ServiceBus.EnableInMemoryTransport();
            registry.AlterSettings<LightningQueueSettings>(x => x.DisableIfNoChannels = true);

            using (var runtime = registry.ToRuntime())
            {
                var bus = runtime.Get<IServiceBus>();

                bus.Consume(new TracedInput { Fail = true });
                

                var history = runtime.Get<IChainExecutionHistory>();

                var log = history.RecentReports().Single(x => x.RootChain != null && x.RootChain.InputType() == typeof(TracedInput));

                log.Steps.Any(x => x.Log is ExceptionReport).ShouldBeTrue();
                log.HadException.ShouldBeTrue();
            }
        }

        [Test]
        public void see_tracing_logs_in_verbose_mode_happy_path()
        {
            var registry = new FubuRegistry();
            registry.ServiceBus.Enable(true);
            registry.Features.Diagnostics.Enable(TraceLevel.Verbose);
            registry.ServiceBus.EnableInMemoryTransport();
            registry.AlterSettings<LightningQueueSettings>(x => x.DisableIfNoChannels = true);

            using (var runtime = registry.ToRuntime())
            {
                var bus = runtime.Get<IServiceBus>();

                bus.Consume(new TracedInput());

                var history = runtime.Get<IChainExecutionHistory>();

                var log = history.RecentReports().Single(x => x.RootChain != null && x.RootChain.InputType() == typeof(TracedInput));

                log.Request["headers"].ShouldBeOfType<Dictionary<string, object>>();
                
                log.Steps.Any().ShouldBeTrue();
                log.Steps.Any(x => x.Log is StringMessage).ShouldBeTrue();

                log.Steps.Each(x => Debug.WriteLine(x));
            }
        }
    }

    public class TracedInput
    {
        public bool Fail { get; set; }
    }

    public class TracedInputConsumer
    {
        private readonly ILogger _logger;

        public TracedInputConsumer(ILogger logger)
        {
            _logger = logger;
        }

        public void Consume(TracedInput input)
        {
            _logger.Info("I got into TracedInputConsumer");

            Thread.Sleep(1.Seconds());
            if (input.Fail)
            {
                throw new DivideByZeroException("You shall not pass!");
            }
        }
    }
}