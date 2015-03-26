using System;
using FubuCore;
using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;
using Serenity.WebDriver.EmbeddedDrivers;
using StoryTeller;

namespace Serenity
{
    public class PhantomBrowser : EmbeddedBrowserLifecycle<PhantomEmbeddedDriver>
    {
        public const string Process = "phantomjs";
        public const string File = "phantomjs.exe";

        public override string BrowserName { get { return "Phantom"; } }

        protected override IWebDriver constructDriver()
        {
            return new PhantomJSDriver(AppDomain.CurrentDomain.BaseDirectory);
        }

        protected override void aggressiveCleanup()
        {
            Kill.Processes(Process, File);
        }
    }
}