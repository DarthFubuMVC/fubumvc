using System;
using FubuCore;
using FubuMVC.Core.Endpoints;
using FubuMVC.Core.Urls;
using NUnit.Framework;
using OpenQA.Selenium;
using Serenity.Fixtures;
using StoryTeller.Engine;
using By = Serenity.WebDriver.By;
using TestContext = StoryTeller.Engine.TestContext;

namespace Serenity.Testing.WebDriver
{
    // SAMPLE: simpleConfigureByJQuery
    public class ByJQueryConfigureExample : ScreenFixture
    {
        [FormatAs("Some grammar that clicks an element")]
        public void ClickButtonSimple()
        {
            var by = By.jQuery(".button-class");
            by.CheckForJQuery = true;
            by.JQueryCheckCount = 3;
            by.JQueryCheckInterval = TimeSpan.FromMilliseconds(100.0);
            Driver.FindElement((By) by).Click();
        }
    }
    // ENDSAMPLE

    public class ByJQueryConfigureScriptTimeoutExample : ScreenFixture
    {
        // SAMPLE: simpleConfigureScriptTimeout
        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            var lifecycle = BrowserForTesting.Use<ChromeBrowser>();

            var context = new TestContext();

            var applicationUnderTest = new StubbedApplicationUnderTest
            {
                Browser = lifecycle
            };

            applicationUnderTest.Driver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromMilliseconds(1000));

            context.Store<IApplicationUnderTest>(applicationUnderTest);

            SetUp(context);
        }
        // ENDSAMPLE

        private class StubbedApplicationUnderTest : IApplicationUnderTest
        {
            public string Name { get; set; }
            public IUrlRegistry Urls { get; set; }

            public IWebDriver Driver { get { return Browser.Driver; } }

            public string RootUrl { get; set; }
            public IServiceLocator Services { get; set; }
            public IBrowserLifecycle Browser { get; set; }

            public void Ping()
            {
                throw new System.NotSupportedException();
            }

            public void Teardown()
            {
                throw new System.NotSupportedException();
            }

            public NavigationDriver Navigation { get; set; }

            public EndpointDriver Endpoints()
            {
                throw new System.NotSupportedException();
            }
        }
    }
}
