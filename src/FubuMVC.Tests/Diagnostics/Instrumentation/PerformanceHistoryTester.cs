using FubuMVC.Core.Diagnostics.Instrumentation;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.Diagnostics.Instrumentation
{
    [TestFixture]
    public class PerformanceHistoryTester
    {
        [Test]
        public void read_increments_hit_count()
        {
            var history = new PerformanceHistory();

            for (var i = 0; i < 10; i++)
            {
                history.Read(new StubRequestLog());
                history.HitCount.ShouldBe(i + 1);
            }
        }

        [Test]
        public void read_stores_the_last_execution()
        {
            var history = new PerformanceHistory();
            var log1 = new ChainExecutionLog();
            var log2 = new ChainExecutionLog();

            history.Read(log1);
            history.LastExecution.ShouldBeTheSameAs(log1);

            history.Read(log2);
            history.LastExecution.ShouldBeTheSameAs(log2);
        }

        [Test]
        public void read_increments_exception_count_correctly()
        {
            var history = new PerformanceHistory();
            history.Read(new StubRequestLog{HadException = false});
            history.ExceptionCount.ShouldBe(0);

            history.Read(new StubRequestLog { HadException = true });
            history.ExceptionCount.ShouldBe(1);

            history.Read(new StubRequestLog { HadException = true });
            history.ExceptionCount.ShouldBe(2);

            history.Read(new StubRequestLog { HadException = false });
            history.ExceptionCount.ShouldBe(2);
        }

        [Test]
        public void increment_total_time()
        {
            var history = new PerformanceHistory();

            history.Read(new StubRequestLog{ExecutionTime = 100});
            history.Read(new StubRequestLog{ExecutionTime = 200});
            history.Read(new StubRequestLog{ExecutionTime = 50});
            history.Read(new StubRequestLog{ExecutionTime = 70});

            history.TotalExecutionTime.ShouldBe(100 + 200 + 50 + 70);
        }

        [Test]
        public void calculate_average()
        {
            var history = new PerformanceHistory();

            history.Read(new StubRequestLog { ExecutionTime = 100 });
            history.Read(new StubRequestLog { ExecutionTime = 200 });
            history.Read(new StubRequestLog { ExecutionTime = 50 });
            history.Read(new StubRequestLog { ExecutionTime = 70 });

            history.Average.ShouldBe((100 + 200 + 50 + 70) / 4);
        }

        [Test]
        public void no_hits_means_zero_exception_percentage_and_zero_average()
        {
            var history = new PerformanceHistory();
            history.Average.ShouldBe(0);
            history.ExceptionPercentage.ShouldBe(0);
        }

        [Test]
        public void calculate_exception_percentage()
        {
            var history = new PerformanceHistory();
            

            history.Read(new StubRequestLog { ExecutionTime = 100 });
            history.Read(new StubRequestLog { ExecutionTime = 200, HadException = true});
            history.Read(new StubRequestLog { ExecutionTime = 50 });
            history.Read(new StubRequestLog { ExecutionTime = 70, HadException = true});
            history.Read(new StubRequestLog { ExecutionTime = 70 });

            history.ExceptionPercentage.ShouldBe(40);
        }

        [Test]
        public void track_min_time()
        {
            var history = new PerformanceHistory();

            history.Read(new StubRequestLog { ExecutionTime = 100 });
            history.MinTime.ShouldBe(100);

            history.Read(new StubRequestLog { ExecutionTime = 200 });
            history.MinTime.ShouldBe(100);

            history.Read(new StubRequestLog { ExecutionTime = 50 });
            history.MinTime.ShouldBe(50);

            history.Read(new StubRequestLog { ExecutionTime = 70 });
            history.MinTime.ShouldBe(50);
        }

        [Test]
        public void track_max_time()
        {
            var history = new PerformanceHistory();

            history.Read(new StubRequestLog { ExecutionTime = 100 });
            history.MaxTime.ShouldBe(100);

            history.Read(new StubRequestLog { ExecutionTime = 200 });
            history.MaxTime.ShouldBe(200);

            history.Read(new StubRequestLog { ExecutionTime = 50 });
            history.MaxTime.ShouldBe(200);

            history.Read(new StubRequestLog { ExecutionTime = 70 });
            history.MaxTime.ShouldBe(200);
        }
    }

    public class StubRequestLog : IRequestLog
    {
        public double ExecutionTime { get; set; }
        public bool HadException { get; set; }
    }
}