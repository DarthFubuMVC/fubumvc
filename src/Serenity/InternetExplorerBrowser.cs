using OpenQA.Selenium;
using OpenQA.Selenium.IE;

namespace Serenity
{
    public class InternetExplorerBrowser : BrowserLifecycle
    {
        public const string Process = "IEXPLORE";

        public override string BrowserName { get { return "Internet Explorer"; } }

        protected override IWebDriver buildDriver()
        {
            return new InternetExplorerDriver();
        }

        protected override void aggressiveCleanup()
        {
            Kill.Processes(Process);
        }
    }
}