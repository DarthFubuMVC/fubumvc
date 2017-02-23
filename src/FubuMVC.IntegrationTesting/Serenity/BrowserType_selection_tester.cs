using FubuCore;
using Xunit;
using Serenity;
using Shouldly;
using StoryTeller;
using StoryTeller.Engine;

namespace FubuMVC.IntegrationTesting.Serenity
{
    
    public class BrowserType_selection_tester
    {

        [Fact]
        public void select_with_no_profile_or_default_should_be_chrome()
        {
            Project.CurrentProject = new Project{Profile = null};
            BrowserFactory.DetermineBrowserType(null)
                .ShouldBe(BrowserType.Chrome);

        }

        [Fact]
        public void select_with_no_profile_and_a_default()
        {
            Project.CurrentProject = new Project { Profile = null };
            BrowserFactory.DetermineBrowserType(BrowserType.Firefox)
                .ShouldBe(BrowserType.Firefox);
        }

        [Fact]
        public void profile_wins()
        {
            Project.CurrentProject = new Project { Profile = "phantom" };

            BrowserFactory.DetermineBrowserType(null)
                .ShouldBe(BrowserType.Phantom);

            BrowserFactory.DetermineBrowserType(BrowserType.Firefox)
                .ShouldBe(BrowserType.Phantom);

        }

        [Fact]
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

        [Fact]
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

        [Fact]
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