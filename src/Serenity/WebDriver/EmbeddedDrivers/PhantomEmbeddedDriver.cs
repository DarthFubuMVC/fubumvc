using System;

namespace Serenity.WebDriver.EmbeddedDrivers
{
    public class PhantomEmbeddedDriver : IEmbeddedDriver
    {
        private Version _version;
        public Version Version { get { return _version ?? (_version = new Version("1.9.2.0")); } }
        public string ResourceName { get { return "phantomjs.exe"; } }
        public string ExtractedFileName { get { return "phantomjs.exe"; } }
    }
}