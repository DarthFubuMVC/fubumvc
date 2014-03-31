using Fubu.Running;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace fubu.Testing.Running
{
    [TestFixture]
    public class FubuMvcApplicationFileWatcherTester : InteractionContext<FubuMvcApplicationFileWatcher>
    {
        private const string theFile = "something.txt";

        [Test]
        public void get_a_file_that_does_nohting()
        {
            MockFor<IFileMatcher>().Stub(x => x.CategoryFor(theFile)).Return(FileChangeCategory.Nothing);

            ClassUnderTest.ChangeFile(theFile);

            MockFor<IApplicationObserver>().AssertWasNotCalled(x => x.RecycleAppDomain());
            MockFor<IApplicationObserver>().AssertWasNotCalled(x => x.RecycleApplication());
            MockFor<IApplicationObserver>().AssertWasNotCalled(x => x.RefreshContent());
        }

        [Test]
        public void get_a_file_that__triggers_app_domain_recycle()
        {
            MockFor<IFileMatcher>().Stub(x => x.CategoryFor(theFile)).Return(FileChangeCategory.AppDomain);

            ClassUnderTest.ChangeFile(theFile);

            MockFor<IApplicationObserver>().AssertWasCalled(x => x.RecycleAppDomain());
            MockFor<IApplicationObserver>().AssertWasNotCalled(x => x.RecycleApplication());
            MockFor<IApplicationObserver>().AssertWasNotCalled(x => x.RefreshContent());
        }

        [Test]
        public void get_a_file_that_should_trigger_application_recycle()
        {
            MockFor<IFileMatcher>().Stub(x => x.CategoryFor(theFile)).Return(FileChangeCategory.Application);

            ClassUnderTest.ChangeFile(theFile);

            MockFor<IApplicationObserver>().AssertWasNotCalled(x => x.RecycleAppDomain());
            MockFor<IApplicationObserver>().AssertWasCalled(x => x.RecycleApplication());
            MockFor<IApplicationObserver>().AssertWasNotCalled(x => x.RefreshContent());
        }

        [Test]
        public void get_a_file_that_should_trigger_content_refresh()
        {
            MockFor<IFileMatcher>().Stub(x => x.CategoryFor(theFile)).Return(FileChangeCategory.Content);

            ClassUnderTest.ChangeFile(theFile);

            MockFor<IApplicationObserver>().AssertWasNotCalled(x => x.RecycleAppDomain());
            MockFor<IApplicationObserver>().AssertWasNotCalled(x => x.RecycleApplication());
            MockFor<IApplicationObserver>().AssertWasCalled(x => x.RefreshContent());
        }
    }
}