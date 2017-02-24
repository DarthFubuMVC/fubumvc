using System;
using FubuCore;
using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;

namespace Serenity
{
    public class PhantomBrowser : BrowserLifecycle
    {
        public const string Process = "phantomjs";
        public const string File = "phantomjs.exe";

        public override string BrowserName => "Phantom";

        protected override void aggressiveCleanup()
        {
            Kill.Processes(Process, File);
        }

        protected override IWebDriver buildDriver()
        {
            return new PhantomJSDriver();
        }
    }
}