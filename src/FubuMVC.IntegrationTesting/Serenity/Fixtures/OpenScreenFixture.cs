using System.Collections.Generic;
using System.Linq;
using FubuCore;
using Serenity.Fixtures;
using StoryTeller;
using StoryTeller.Grammars.Tables;

namespace FubuMVC.IntegrationTesting.Serenity.Fixtures
{
    public class OpenScreenFixture : ScreenFixture
    {
        [ExposeAsTable("Open Screens without Error")]
        [FormatAs("Can open browser without error to {Url}")]
        public bool OpenScreens(string Url)
        {
            Navigation.NavigateToUrl(Url);

            var errors = JavascriptErrorsInBrowser();
            StoryTellerAssert.Fail(errors.Any(x => x.IsNotEmpty()), () =>
            {
                var text = "JS errors detected: " + errors.Join("; ");

                return text;
            });

            return true;
        }         
    }
}