using FubuCore;
using FubuMVC.Authentication.Serenity;
using StoryTeller;

namespace Specifications.Fixtures
{
    public class OtherScreenFixture : LoginScreenFixture
    {
        [FormatAs("Go to a different page for {name}")]
        public void GoToDifferentPage(string name)
        {
            Navigation.NavigateTo(new DifferentInput{Name = name});
        }

        [FormatAs("Should be on the different page for {name}")]
        public bool ShouldBeOnTheDifferentPage(string name)
        {
            var url = Urls.UrlFor(new DifferentInput {Name = name});
            return Browser.Driver.Url.EqualsIgnoreCase(url);
        }
    }
}