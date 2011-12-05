using KayakTestApplication;
using NUnit.Framework;
using TestContext = StoryTeller.Engine.TestContext;
using FubuTestingSupport;

namespace Serenity.Testing
{
    [TestFixture]
    public class InProcessSerenitySystemTester
    {
        private ApplicationDriver theDriver;
        private InProcessSerenitySystem<KayakApplication> theSystem;

        [SetUp]
        public void SetUp()
        {
            var context = new TestContext();
            theSystem = new InProcessSerenitySystem<KayakApplication>();
            theSystem.SetupEnvironment();
            theSystem.RegisterServices(context);

            theDriver = context.Retrieve<ApplicationDriver>();
        }

        [TearDown]
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
    }
}