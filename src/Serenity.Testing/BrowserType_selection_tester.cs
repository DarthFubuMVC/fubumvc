using FubuMVC.Core;
using Shouldly;
using NUnit.Framework;
using StoryTeller;

namespace Serenity.Testing
{
    [TestFixture]
    public class BrowserType_selection_tester
    {
        private FubuMvcSystem theSystem;

        [SetUp]
        public void SetUp()
        {
            theSystem = new FubuMvcSystem(() => { return null; });
            theSystem.DefaultBrowser = BrowserType.Chrome;
        }

        [Test]
        public void use_profile_if_it_exists()
        {
            Project.CurrentProject = new Project{Profile = BrowserType.Phantom.ToString()};

            theSystem.ChooseBrowserType().ShouldBe(BrowserType.Phantom);
        }

        [Test]
        public void use_the_project_default_if_it_exists_and_no_profile()
        {
            Project.CurrentProject = new Project{Profile = null};
            WebDriverSettings.Current.Browser = BrowserType.Chrome;

            theSystem.DefaultBrowser = BrowserType.IE;

            theSystem.ChooseBrowserType().ShouldBe(BrowserType.IE);
        }

        [Test]
        public void fall_back_to_the_webdriver_settings_current_if_no_profile_or_default()
        {
            Project.CurrentProject = new Project { Profile = null };
            WebDriverSettings.Current.Browser = BrowserType.Chrome;

            theSystem.DefaultBrowser = null;

            theSystem.ChooseBrowserType().ShouldBe(BrowserType.Chrome);
        }

    }
}