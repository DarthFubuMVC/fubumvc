using System.Reflection;
using FubuCore;
using NUnit.Framework;
using Shouldly;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;
using OpenQA.Selenium.PhantomJS;

namespace Serenity.Testing
{
    [TestFixture]
    public class WebDriverSettingsTester
    {

        [Test]
        public void read_website_settings_if_there_is_no_file()
        {
            new FileSystem().DeleteFile(WebDriverSettings.Filename);

            WebDriverSettings.Read().Browser.ShouldBe(new WebDriverSettings().Browser);
        }

        [Test]
        public void read_website_settings()
        {
            new FileSystem().AlterFlatFile(WebDriverSettings.Filename, list => list.Add("WebDriverSettings.Browser=IE"));

            WebDriverSettings.Read().Browser.ShouldBe(BrowserType.IE);
        }



        [Test]
        public void default_browser_is_firefox()
        {
            // It's this way because the web driver for firefox works best on my
            // box and I'm the one who wants this to work quickly today
            // - Jeremy 10/5/2011

            new WebDriverSettings().Browser.ShouldBe(BrowserType.Firefox);
        }

        [Test]
        public void build_firefox_driver()
        {
            using (var browser = WebDriverSettings.GetBrowserLifecyle(BrowserType.Firefox))
            {
                browser.ShouldBeOfType<FirefoxBrowser>();
            }
        }

        [Test]
        public void build_chrome_driver()
        {
            using (var browser = WebDriverSettings.GetBrowserLifecyle(BrowserType.Chrome))
            {
                browser.ShouldBeOfType<ChromeBrowser>();
            }
        }


        [Test]
        public void build_phantom_driver()
        {
            using (var browser = WebDriverSettings.GetBrowserLifecyle(BrowserType.Phantom))
            {
                browser.ShouldBeOfType<PhantomBrowser>();
            }
        }

        // Before you run this, make sure that IE opens without the F12 tools visible because
        // WebDriver things that's a modal dialog box and throws up
        [Test, Ignore("Need to do the IE Driver thing like Chrome")]
        public void build_IE_driver_because_you_know_you_will_have_to_do_this_at_some_point()
        {
            using (var browser = WebDriverSettings.GetBrowserLifecyle(BrowserType.IE))
            {
                browser.ShouldBeOfType<InternetExplorerDriver>();
            }

        }
    }
}