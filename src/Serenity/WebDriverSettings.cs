using System;
using FubuCore.Configuration;

namespace Serenity
{
    public class WebDriverSettings
    {
        public static readonly string Filename = "browser.settings.config";
        private static readonly Lazy<WebDriverSettings> _settings = new Lazy<WebDriverSettings>(Read);

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

        public static WebDriverSettings Current
        {
            get { return _settings.Value; }
        }

        public static IBrowserLifecycle GetBrowserLifecyle()
        {
            return GetBrowserLifecyle(Current.Browser);
        }

        public static IBrowserLifecycle GetBrowserLifecyle(BrowserType browserType)
        {
            switch (browserType)
            {
                case BrowserType.Chrome:
                    return new ChromeBrowser();

                case BrowserType.IE:
                    return new InternetExplorerBrowser();

                case BrowserType.Firefox:
                    return new FirefoxBrowser();

                case BrowserType.Phantom:
                    return new PhantomBrowser();

                default:
                    throw new ArgumentOutOfRangeException("Unrecognized browser");
            }
        }

        public static void Import(SerenityEnvironment settings)
        {
            Current.Browser = settings.Browser;
        }
    }
}