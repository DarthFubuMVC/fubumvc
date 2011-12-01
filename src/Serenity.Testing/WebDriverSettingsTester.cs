using System.Reflection;
using FubuCore;
using NUnit.Framework;
using FubuTestingSupport;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;

namespace Serenity.Testing
{
    [TestFixture]
    public class WebDriverSettingsTester
    {

        [Test]
        public void read_website_settings_if_there_is_no_file()
        {
            new FileSystem().DeleteFile(WebDriverSettings.Filename);

            WebDriverSettings.Read().Browser.ShouldEqual(new WebDriverSettings().Browser);
        }

        [Test]
        public void read_website_settings()
        {
            new FileSystem().AlterFlatFile(WebDriverSettings.Filename, list => list.Add("WebDriverSettings.Browser=IE"));

            WebDriverSettings.Read().Browser.ShouldEqual(BrowserType.IE);
        }



        [Test]
        public void default_browser_is_firefox()
        {
            // It's this way because the web driver for firefox works best on my
            // box and I'm the one who wants this to work quickly today
            // - Jeremy 10/5/2011

            new WebDriverSettings().Browser.ShouldEqual(BrowserType.Firefox);
        }

        [Test]
        public void build_firefox_driver()
        {
            using (var browser = WebDriverSettings.DriverBuilder(BrowserType.Firefox)())
            {
                browser.ShouldBeOfType<FirefoxDriver>();
                browser.Close();
            }
            
        }

        [Test]
        public void build_chrome_driver()
        {
            using (var browser = WebDriverSettings.DriverBuilder(BrowserType.Chrome)())
            {
                browser.ShouldBeOfType<ChromeDriver>();
                browser.Close();
            }
        }

        [Test]
        public void build_IE_driver_because_you_know_you_will_have_to_do_this_at_some_point()
        {
            using (var browser = WebDriverSettings.DriverBuilder(BrowserType.IE)())
            {
                browser.ShouldBeOfType<InternetExplorerDriver>();
                browser.Close();
            }

        }
    }
}