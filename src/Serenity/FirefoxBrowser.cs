using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace Serenity
{
    public class FirefoxBrowser : BrowserLifecycle
    {
        public const string Process = "firefox";

        public override string BrowserName { get { return "FireFox"; } }

        protected override IWebDriver buildDriver()
        {
            return new FirefoxDriver();
        }

        protected override void aggressiveCleanup()
        {
            Kill.Processes(Process);
        }
    }
}