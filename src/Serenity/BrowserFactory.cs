using System;
using FubuCore;
using StoryTeller;

namespace Serenity
{
    public static class BrowserFactory
    {
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

        public static BrowserType DetermineBrowserType(BrowserType? @default)
        {
            return selectForProfile() ?? @default ?? BrowserType.Chrome;
        }

        private static BrowserType? selectForProfile()
        {
            if (Project.CurrentProfile.IsEmpty()) return null;

            var parts = Project.CurrentProfile.Split('/');

            BrowserType browser;
            foreach (var part in parts)
            {
                if (Enum.TryParse(part, true, out browser))
                {
                    return browser;
                }
            }

            return null;
        }
    }
}