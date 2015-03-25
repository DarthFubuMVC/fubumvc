using System;

namespace Serenity.WebDriver.EmbeddedDrivers
{
    public class ChromeEmbeddedDriver : IEmbeddedDriver
    {
        private Version _version;
        public Version Version { get { return _version ?? (_version = new Version("2.0")); } }
        public string ResourceName { get { return "chromedriver.exe"; } }
        public string ExtractedFileName { get { return "chromedriver.exe"; } }
    }
}