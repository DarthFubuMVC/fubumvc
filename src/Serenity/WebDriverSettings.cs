using System;
using FubuCore.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;

namespace Serenity
{
    public class WebDriverSettings
    {
        public static readonly string Filename = "browser.settings"; 

        public WebDriverSettings()
        {
            // Why is the default Firefox you ask?  Because it's the first one that Jeremy got working on his box.
            Browser = BrowserType.Firefox;
        }

        public BrowserType Browser { get; set; }

        public static WebDriverSettings Read()
        {
            var settings = SettingsData.ReadFromFile(SettingCategory.core, Filename);
            return SettingsProvider.For(settings).SettingsFor<WebDriverSettings>();
        }

        public Func<IWebDriver> DriverBuilder()
        {
            switch (Browser)
            {
                case BrowserType.Chrome:
                    return () => new ChromeDriver();

                case BrowserType.IE:
                    return () => new InternetExplorerDriver();

                case BrowserType.Firefox:
                    return () => new FirefoxDriver();

                default:
                    throw new ArgumentOutOfRangeException("Unrecognized browser");
            }
        }
    }
}