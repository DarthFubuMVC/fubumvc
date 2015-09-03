using FubuCore;
using NUnit.Framework;
using Serenity;
using Shouldly;
using StoryTeller;
using StoryTeller.Engine;

namespace FubuMVC.IntegrationTesting.Serenity
{
    [TestFixture]
    public class BrowserType_selection_tester
    {

        [Test]
        public void select_with_no_profile_or_default_should_be_chrome()
        {
            Project.CurrentProject = new Project{Profile = null};
            BrowserFactory.DetermineBrowserType(null)
                .ShouldBe(BrowserType.Chrome);

        }

        [Test]
        public void select_with_no_profile_and_a_default()
        {
            Project.CurrentProject = new Project { Profile = null };
            BrowserFactory.DetermineBrowserType(BrowserType.Firefox)
                .ShouldBe(BrowserType.Firefox);
        }

        [Test]
        public void profile_wins()
        {
            Project.CurrentProject = new Project { Profile = "phantom" };

            BrowserFactory.DetermineBrowserType(null)
                .ShouldBe(BrowserType.Phantom);

            BrowserFactory.DetermineBrowserType(BrowserType.Firefox)
                .ShouldBe(BrowserType.Phantom);

        }

        [Test]
        public void build_for_a_system_all_defaults()
        {
            Project.CurrentProject = new Project { Profile = null };
            using (var system = new SerenitySystem())
            {
                system.As<ISystem>().Warmup().Wait();

                system.Runtime.Get<IBrowserLifecycle>()
                    .ShouldBeOfType<ChromeBrowser>();
            }
        }

        [Test]
        public void build_for_a_system_with_default()
        {
            Project.CurrentProject = new Project { Profile = null };
            using (var system = new SerenitySystem{DefaultBrowser = BrowserType.Firefox})
            {
                system.As<ISystem>().Warmup().Wait();

                system.Runtime.Get<IBrowserLifecycle>()
                    .ShouldBeOfType<FirefoxBrowser>();
            }
        }

        [Test]
        public void build_with_system_for_profile()
        {
            Project.CurrentProject = new Project { Profile = "phantom/katana" };
            using (var system = new SerenitySystem { DefaultBrowser = BrowserType.Firefox })
            {
                system.As<ISystem>().Warmup().Wait();

                system.Runtime.Get<IBrowserLifecycle>()
                    .ShouldBeOfType<PhantomBrowser>();
            }
        }
    }
}