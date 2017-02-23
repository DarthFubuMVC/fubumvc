﻿using System;
using System.Linq;
using System.Threading;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Events;
using FubuMVC.Core.ServiceBus.Polling;
using FubuMVC.Tests.TestSupport;
using Xunit;
using Rhino.Mocks;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Events
{
    
    public class ExpiringListenerCleanupTester : InteractionContext<ExpiringListenerCleanup>
    {
        [Fact]
        public void execute_prunes_for_the_current_time()
        {
            LocalSystemTime = DateTime.Today.AddHours(8);

            ClassUnderTest.Execute(new CancellationToken());

            MockFor<IEventAggregator>().AssertWasCalled(x => x.PruneExpiredListeners(UtcSystemTime));
        }
    }

    
    public class Expiring_listener_polling_job_is_registered
    {
        [Fact]
        public void the_cleanup_job_is_registered()
        {
            using (var runtime = FubuRuntime.BasicBus()
                )
            {
                runtime.Get<IPollingJobs>()
                    .Any(x => x is PollingJob<ExpiringListenerCleanup, TransportSettings>)
                    .ShouldBeTrue();
            }
        }
    }
}