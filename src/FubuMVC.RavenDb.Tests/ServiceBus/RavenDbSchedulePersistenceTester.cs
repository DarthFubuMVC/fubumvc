using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Persistence;
using FubuMVC.RavenDb.ServiceBus;
using NUnit.Framework;
using Shouldly;
using StructureMap;

namespace FubuMVC.RavenDb.Tests.ServiceBus
{
    [TestFixture]
    public class RavenDbSchedulePersistenceTester
    {
        private FubuRuntime runtime;
        private RavenDbSchedulePersistence thePersistence;

        private JobStatusDTO foo1;
        private JobStatusDTO foo2;
        private JobStatusDTO foo3;
        private JobStatusDTO bar1;
        private JobStatusDTO bar2;

        [SetUp]
        public void SetUp()
        {
            runtime = FubuRuntime.BasicBus();
            runtime.Get<IContainer>().UseInMemoryDatastore();

            thePersistence = runtime.Get<RavenDbSchedulePersistence>();

            foo1 = new JobStatusDTO {JobKey = "1", NodeName = "foo"};
            foo2 = new JobStatusDTO {JobKey = "2", NodeName = "foo"};
            foo3 = new JobStatusDTO {JobKey = "3", NodeName = "foo"};
            bar1 = new JobStatusDTO {JobKey = "1", NodeName = "bar"};
            bar2 = new JobStatusDTO {JobKey = "2", NodeName = "bar"};

            thePersistence.Persist(new[] {foo1, foo2, foo3, bar1, bar2});
        }

        [TearDown]
        public void TearDown()
        {
            runtime.Dispose();
        }


        [Test]
        public void store_history()
        {
            var record1 = new JobExecutionRecord();
            var record2 = new JobExecutionRecord();
            var record3 = new JobExecutionRecord();
            var record4 = new JobExecutionRecord();

            thePersistence.RecordHistory("foo", "1", record1);
            thePersistence.RecordHistory("foo", "1", record2);
            thePersistence.RecordHistory("foo", "2", record3);
            thePersistence.RecordHistory("foo", "2", record4);

            thePersistence.FindHistory("foo", "1").ShouldHaveTheSameElementsAs(record1, record2);
            thePersistence.FindHistory("foo", "2").ShouldHaveTheSameElementsAs(record3, record4);
        }

        [Test]
        public void find_all_for_node()
        {
            thePersistence.FindAll("foo")
                .ShouldHaveTheSameElementsAs(foo1, foo2, foo3);

            thePersistence.FindAll("bar")
                .ShouldHaveTheSameElementsAs(bar1, bar2);
        }

        [Test]
        public void find_all_active_for_node()
        {
            foo1.Status = foo2.Status = bar1.Status = JobExecutionStatus.Scheduled;
            foo3.Status = bar2.Status = JobExecutionStatus.Inactive;

            thePersistence.Persist(new[] {foo1, foo2, foo3, bar1, bar2});

            thePersistence.FindAllActive("foo")
                .ShouldHaveTheSameElementsAs(foo1, foo2);

            thePersistence.FindAllActive("bar")
                .ShouldHaveTheSameElementsAs(bar1);
        }

        [Test]
        public void persist_job_status()
        {
            foo1.Status = foo2.Status = foo3.Status = bar1.Status = bar2.Status = JobExecutionStatus.Inactive;
            thePersistence.Persist(new[] {foo1, foo2, foo3, bar1, bar2});

            var change = new JobStatusDTO {JobKey = "1", NodeName = "foo", Status = JobExecutionStatus.Scheduled};

            thePersistence.Persist(change);

            thePersistence.FindAllActive("foo")
                .ShouldHaveTheSameElementsAs(change);

            thePersistence.Find("foo", "1").Status.ShouldBe(change.Status);
        }

        [Test]
        public void find_a_single_status()
        {
            thePersistence.Find("foo", "1")
                .Id.ShouldBe("foo/1");
        }
    }
}