using System;
using System.Threading;
using FubuCore;
using FubuMVC.Core.ServiceBus.Polling;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Polling
{
    [TestFixture]
    public class JobTimeoutTester
    {
        [Test]
        public void run_to_completion_within_the_time()
        {
            var timeout = new JobTimeout(5.Seconds());
            var job = new FakeJob(1.Seconds());

            var task = timeout.Execute(job);
            task.Wait();

            task.IsCanceled.ShouldBeFalse();
            task.IsFaulted.ShouldBeFalse();
            task.Exception.ShouldBeNull();

            job.Finished.ShouldBeTrue();
        }

        [Test]
        public void run_over_the_allowed_time()
        {
            var timeout = new JobTimeout(1.Seconds());
            var job = new FakeJob(10.Seconds());

            var task = timeout.Execute(job);

            Exception<AggregateException>.ShouldBeThrownBy(() => {
                task.Wait();
            });



            task.Exception.InnerException.ShouldBeOfType<TimeoutException>();
            job.Finished.ShouldBeFalse();
        }
    }

    public class FakeJob : IJob
    {
        private readonly TimeSpan _timeToRun;
        public bool WasCancelled;
        public bool Finished;

        public FakeJob(TimeSpan timeToRun)
        {
            _timeToRun = timeToRun;
        }

        public void Execute(CancellationToken cancellation)
        {
            var ending = DateTime.UtcNow.Add(_timeToRun);
            while (DateTime.UtcNow <= ending)
            {
                Thread.Sleep(100);
                if (cancellation.IsCancellationRequested)
                {
                    WasCancelled = true;
                    cancellation.ThrowIfCancellationRequested();
                }
            }

            Finished = true;
        }
    }
}