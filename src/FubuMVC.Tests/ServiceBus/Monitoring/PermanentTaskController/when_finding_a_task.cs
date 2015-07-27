using FubuMVC.Core.ServiceBus;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.Tests.ServiceBus.Monitoring.PermanentTaskController
{
    [TestFixture]
    public class when_finding_a_task : PersistentTaskControllerContext
    {
        private FakePersistentTask existingSubject1;
        private FakePersistentTask existingSubject2;

        protected override void theContextIs()
        {
            existingSubject1 = sources["foo"].AddTask("a");
            sources["foo"].AddTask("b");
            sources["bar"].AddTask("c");
            existingSubject2 = sources["bar"].AddTask("b");
        }

        [Test]
        public void happy_path()
        {
            theController.FindTask(existingSubject1.Subject)
                .ShouldBeTheSameAs(existingSubject1);

            theController.FindTask(existingSubject2.Subject)
                .ShouldBeTheSameAs(existingSubject2);

        }

        [Test]
        public void sad_path_unknown_protocol()
        {
            theController.FindTask("wrong://1".ToUri())
                .ShouldBeNull();
        }

        [Test]
        public void sad_path_unknown_subject_for_the_task()
        {
            theController.FindTask("foo://wrong".ToUri())
                .ShouldBeNull();
        }
    }
}