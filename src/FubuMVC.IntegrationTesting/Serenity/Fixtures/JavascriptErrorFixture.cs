using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.IntegrationTesting.Serenity.App;
using Serenity.Fixtures;
using StoryTeller;

namespace FubuMVC.IntegrationTesting.Serenity.Fixtures
{
    public class JavascriptErrorFixture : ScreenFixture
    {
        public JavascriptErrorFixture()
        {
            Title = "In a page with a javascript error";
        }

        protected override void beforeRunning()
        {
            DisableAllSecurity();
            Navigation.NavigateTo<AppErrorEndpoint>(x => x.get_javascript_error());
        }

        [FormatAs("The page should have a javascript error with the text {text}")]
        public bool HasError(string text)
        {
            var errorsInBrowser = JavascriptErrorsInBrowser();

            StoryTellerAssert.Fail(!errorsInBrowser.Any(), "There were no javascript errors detected in the browser");

            StoryTellerAssert.Fail(!errorsInBrowser.Any(x => x.Contains(text)), () =>
            {
                return "The actual javascript errors were:\n" + errorsInBrowser.Join("\n");
            });

            return true;
        }
    }
}