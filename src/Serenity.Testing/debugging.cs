using System.Threading;
using FubuMVC.Core;
using KayakTestApplication;
using NUnit.Framework;
using TestContext = StoryTeller.Engine.TestContext;

namespace Serenity.Testing
{
    [TestFixture, Explicit]
    public class debugging
    {
        [Test]
        public void write_source()
        {
            var settings = ApplicationSettings.For<KayakTestApplication.KayakApplication>();
            settings.Write();
        }

        [Test]
        public void start_an_inprocess_system_without_blowing_up()
        {
            var context = new TestContext();
            var system = new InProcessSerenitySystem<KayakApplication>();
            system.SetupEnvironment();
            system.RegisterServices(context);

            var driver = context.Retrieve<ApplicationDriver>();

            driver.NavigateToHome();

            Thread.Sleep(5000);

            system.TeardownEnvironment();
        }
    }
}