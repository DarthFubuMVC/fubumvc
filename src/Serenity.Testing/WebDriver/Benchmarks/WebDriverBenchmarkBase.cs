using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework;
using Serenity.Testing.Fixtures;

namespace Serenity.Testing.WebDriver.Benchmarks
{
    public abstract class WebDriverBenchmarkBase<TBrowser, TData> : ScreenManipulationTester<TBrowser> where TBrowser : IBrowserLifecycle, new()
    {
        [Explicit]
        [TestCaseSource("Cases")]
        public void Benchmark(object data)
        {
            IList<TimeSpan> times = new List<TimeSpan>();
            var dataCast = (TData) data;

            var totalTime = Stopwatch.StartNew();

            for (var i = 0; i < 250; i++)
            {
                var stopwatch = Stopwatch.StartNew();
                ActionToBenchmark(dataCast);
                stopwatch.Stop();
                times.Add(stopwatch.Elapsed);
            }

            totalTime.Stop();

            var averageTime = TimeSpan.FromTicks((long) times.Select(x => x.Ticks).Average());

            Console.WriteLine("Average: {0} [Milliseconds: {1:N2}]", averageTime, averageTime.TotalMilliseconds);
            Console.WriteLine("Total: {0}", totalTime.Elapsed);
        }

        protected abstract void ActionToBenchmark(TData data);

        public abstract IEnumerable<TestCaseData> Cases();
    }
}