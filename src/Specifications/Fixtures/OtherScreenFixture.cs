using System.Linq;
using FubuMVC.IntegrationTesting;
using StoryTeller;

namespace Specifications.Fixtures
{
    public class OtherScreenFixture : LoginScreenFixture
    {
        public OtherScreenFixture()
        {
            Title = "Login Screen with a Different Page";
        }

        protected override void beforeRunning()
        {
            Logout();
        }

        [
            FormatAs("Go to a different page for {name}")]
        public void GoToDifferentPage(string name)
        {
            Navigation.NavigateTo(new DifferentInput{Name = name});
        }

        [FormatAs("Should be on the different page for {name}")]
        public string ShouldBeOnTheDifferentPage()
        {
            var currentUrl = Browser.Driver.Url;
            return currentUrl.Split('/').Last();
        }
    }
}