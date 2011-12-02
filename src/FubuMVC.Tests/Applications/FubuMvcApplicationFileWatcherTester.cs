using Fubu.Applications;
using FubuCore;
using FubuTestingSupport;
using NUnit.Framework;
using Rhino.Mocks;

namespace FubuMVC.Tests.Applications
{
    [TestFixture]
    public class FubuMvcApplicationFileWatcherTester : InteractionContext<FubuMvcApplicationFileWatcher>
    {
        private void theContentShouldNotHaveBeenRecycled()
        {
            MockFor<IApplicationDomain>().AssertWasNotCalled(x => x.RecycleContent());
        }

        private void theDomainShouldNotHaveBeenRecycled()
        {
            MockFor<IApplicationDomain>().AssertWasNotCalled(x => x.RecycleDomain());
        }

        private void theDomainShouldHaveBeenRecycled()
        {
            MockFor<IApplicationDomain>().AssertWasCalled(x => x.RecycleDomain());
        }

        private void theContentShouldHaveBeenRecycled()
        {
            MockFor<IApplicationDomain>().AssertWasCalled(x => x.RecycleContent());
        }

        [Test]
        public void change_a_txt_file_should_do_nothing()
        {
            ClassUnderTest.ChangeFile("something.txt");

            theContentShouldNotHaveBeenRecycled();
            theDomainShouldNotHaveBeenRecycled();
        }

        [Test]
        public void change_an_image_file_should_do_nothing()
        {
            ClassUnderTest.ChangeFile("icon.png");

            theContentShouldNotHaveBeenRecycled();
            theDomainShouldNotHaveBeenRecycled();
        }

        [Test]
        public void change_an_assembly_should_recycle_the_app_domain()
        {
            ClassUnderTest.ChangeFile("something.dll");

            theContentShouldNotHaveBeenRecycled();
            theDomainShouldHaveBeenRecycled();
        }

        [Test]
        public void change_an_exe_should_recycle_the_app_domain()
        {
            ClassUnderTest.ChangeFile("something.exe");

            theContentShouldNotHaveBeenRecycled();
            theDomainShouldHaveBeenRecycled();
        }

        [Test]
        public void change_web_config_should_recycle_the_application()
        {
            ClassUnderTest.ChangeFile("web.config");

            theContentShouldNotHaveBeenRecycled();
            theDomainShouldHaveBeenRecycled();
        }

        [Test]
        public void change_anything_under_bin_should_recycle_the_application()
        {
            ClassUnderTest.ChangeFile(FileSystem.Combine("bin", "something.config"));

            theContentShouldNotHaveBeenRecycled();
            theDomainShouldHaveBeenRecycled();
        }

        [Test]
        public void change_a_config_file_that_is_not_web_config_recycles_content()
        {
            ClassUnderTest.ChangeFile("something.config");

            theContentShouldHaveBeenRecycled();
            theDomainShouldNotHaveBeenRecycled();
        }
    }
}