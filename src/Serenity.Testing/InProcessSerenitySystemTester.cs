using System.Diagnostics;
using FubuCore.Conversion;
using FubuTestingSupport;
using KayakTestApplication;
using NUnit.Framework;
using OpenQA.Selenium;
using TestContext = StoryTeller.Engine.TestContext;

namespace Serenity.Testing
{
    [TestFixture]
    public class InProcessSerenitySystemTester
    {
        private NavigationDriver theDriver;
        private InProcessSerenitySystem<KayakApplication> theSystem;
        private IApplicationUnderTest theApplication;

        [TestFixtureSetUp]
        public void SetUp()
        {
            var context = new TestContext();
            theSystem = new InProcessSerenitySystem<KayakApplication>();
            theSystem.Setup();
            theSystem.RegisterServices(context);

            theApplication = theSystem.Get<IApplicationUnderTest>();
            theDriver = theApplication.Navigation;
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            theSystem.TeardownEnvironment();
        }

        [Test]
        public void can_activate_a_spark_screen_proving_that_the_file_system_paths_are_correct()
        {
            theDriver.NavigateToUrl("http://localhost:5500/say/Jeremy");
            theDriver.Driver.FindElement(By.TagName("h1")).Text.ShouldEqual("My name is Jeremy");
        }

        [Test]
        public void can_get_with_no_body()
        {
            theApplication.Endpoints().ReadTextFrom<SayHelloController>(x => x.Hello())
                .ShouldStartWith("Hello");
        }

        [Test]
        public void can_open_the_top_level_page()
        {
            theDriver.NavigateToHome();
            theDriver.Driver.PageSource.ShouldContain("Hello, it's");
        }

        [Test]
        public void can_post()
        {
            var driver = theApplication.Endpoints();
            var response = driver.PostJson(new NameModel{
                Name = "Jeremy"
            });

            Debug.WriteLine(response.ToString());
        }

        [Test]
        public void get_by_type_from_the_system()
        {
            theSystem.Get(typeof (IObjectConverter)).ShouldBeOfType<ObjectConverter>();
        }
    }
}