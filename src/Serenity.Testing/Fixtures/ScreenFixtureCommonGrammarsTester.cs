using System;
using FubuCore;
using FubuMVC.Core.Endpoints;
using FubuMVC.Core.Urls;
using HtmlTags;
using NUnit.Framework;
using OpenQA.Selenium;
using Serenity.Fixtures;
using StoryTeller;
using StoryTeller.Engine;


namespace Serenity.Testing.Fixtures
{
    [TestFixture(typeof(FirefoxBrowser))]
    [TestFixture(typeof(ChromeBrowser))]
    // TODO: Uncomment when we setup the IE Browser
    // [TestFixture(typeof(InternetExplorerBrowser))]
    [TestFixture(typeof(PhantomBrowser))]
    public abstract class ScreenManipulationTester<TBrowser> : ScreenFixture where TBrowser : IBrowserLifecycle, new()
    {
        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            var lifecycle = BrowserForTesting.Use<TBrowser>();

            var simpleExecutionContext = new SimpleExecutionContext();
            Context = new SpecContext(null, new Timings(), new NulloResultObserver(), new StopConditions(), simpleExecutionContext);
            

            var applicationUnderTest = new StubbedApplicationUnderTest
            {
                Browser = lifecycle
            };

            applicationUnderTest.Driver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromMilliseconds(1000));

            simpleExecutionContext.Register<IApplicationUnderTest>(applicationUnderTest);

            SetUp();
        }

        [SetUp]
        public void BeforeTesting()
        {
            new FileSystem().DeleteFile("screenfixture.htm");

            var document = new HtmlDocument();
            ConfigureDocument(document);

            document.WriteToFile("screenfixture.htm");

            Driver.Navigate().GoToUrl("file:///" + "screenfixture.htm".ToFullPath());
            BeforeEach();
        }

        protected abstract void ConfigureDocument(HtmlDocument document);
        protected virtual void BeforeEach() { }

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

    public static class TestingExtensions
    {


        public static void ShouldEqual(this Counts counts, int rights, int wrongs, int exceptions, int syntaxErrors)
        {
            var expected = new Counts
                           {
                               Rights = rights,
                               Wrongs = wrongs,
                               Exceptions = exceptions,
                               SyntaxErrors = syntaxErrors
                           };

            Assert.AreEqual(expected, counts);
        }
    }
}