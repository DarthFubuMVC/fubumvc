using System;
using System.Linq;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Monitoring;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Monitoring
{
    [TestFixture]
    public class TaskHealthResponseTester
    {
        private readonly Uri subject1 = "foo://1".ToUri();
        private readonly Uri subject2 = "foo://2".ToUri();
        private readonly Uri subject3 = "foo://3".ToUri();
        private readonly Uri subject4 = "foo://4".ToUri();

        [Test]
        public void add_missing_subjects_no_change()
        {
            var response = new TaskHealthResponse
            {
                Tasks = new[]
                {
                    new PersistentTaskStatus(subject1, HealthStatus.Active),
                    new PersistentTaskStatus(subject2, HealthStatus.Active),
                    new PersistentTaskStatus(subject3, HealthStatus.Active),
                    new PersistentTaskStatus(subject4, HealthStatus.Active),
                }
            };

            response.AddMissingSubjects(new []{subject1, subject2, subject3, subject4});

            response.Tasks.Count().ShouldBe(4);
        }

        [Test]
        public void add_missing_subjects_with_gaps()
        {
            var response = new TaskHealthResponse
            {
                Tasks = new[]
                {
                    new PersistentTaskStatus(subject1, HealthStatus.Active),
                    //new PersistentTaskStatus(subject2, HealthStatus.Active),
                    //new PersistentTaskStatus(subject3, HealthStatus.Active),
                    new PersistentTaskStatus(subject4, HealthStatus.Active),
                }
            };

            response.AddMissingSubjects(new[] { subject1, subject2, subject3, subject4 });

            response.Tasks.Count().ShouldBe(4);

            response.Tasks.ShouldContain(new PersistentTaskStatus(subject2, HealthStatus.Inactive));
            response.Tasks.ShouldContain(new PersistentTaskStatus(subject3, HealthStatus.Inactive));
        }

        [Test]
        public void build_for_errors()
        {
            var response = TaskHealthResponse.ErrorFor(new[] {subject1, subject2, subject3, subject4});
        
            response.Tasks.ShouldHaveTheSameElementsAs(
                new PersistentTaskStatus(subject1, HealthStatus.Error),
                new PersistentTaskStatus(subject2, HealthStatus.Error),
                new PersistentTaskStatus(subject3, HealthStatus.Error),
                new PersistentTaskStatus(subject4, HealthStatus.Error)
                
                );

            response.ResponseFailed.ShouldBeTrue();
        
        }

    }
}