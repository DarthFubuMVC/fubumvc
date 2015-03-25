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
            var fileSystem = new FileSystem();
            var settings = StoryTellerEnvironment.Get<SerenityEnvironment>();

            if (fileSystem.FileExists(settings.WorkingDir, File))
            {
                return new PhantomJSDriver(settings.WorkingDir);
            }

            return new PhantomJSDriver(AppDomain.CurrentDomain.BaseDirectory);
        }

        protected override void aggressiveCleanup()
        {
            Kill.Processes(Process, File);
        }
    }
}