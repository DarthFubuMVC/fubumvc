using System.Threading;
using KayakTestApplication;
using NUnit.Framework;
using OpenQA.Selenium;
using TestContext = StoryTeller.Engine.TestContext;
using FubuTestingSupport;

namespace Serenity.Testing
{
    [TestFixture]
    public class InProcessSerenitySystemTester
    {
        private ApplicationDriver theDriver;
        private InProcessSerenitySystem<KayakApplication> theSystem;

        [TestFixtureSetUp]
        public void SetUp()
        {
            var context = new TestContext();
            theSystem = new InProcessSerenitySystem<KayakApplication>();
            theSystem.Setup();
            theSystem.RegisterServices(context);

            theDriver = context.Retrieve<ApplicationDriver>();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            theSystem.TeardownEnvironment();
        }

        [Test]
        public void can_open_the_top_level_page()
        {
            theDriver.NavigateToHome();
            theDriver.Driver.PageSource.ShouldContain("Hello, it's");
        }

        [Test]
        public void can_activate_a_spark_screen_proving_that_the_file_system_paths_are_correct()
        {
            theDriver.NavigateToUrl("http://localhost:5500/say/Jeremy");
            theDriver.Driver.FindElement(By.TagName("h1")).Text.ShouldEqual("My name is Jeremy");
        }
    }
}