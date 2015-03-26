using FubuCore;
using OpenQA.Selenium;
using StoryTeller;

namespace Serenity.WebDriver.EmbeddedDrivers
{
    public abstract class EmbeddedBrowserLifecycle<TEmbeddedDriver> : BrowserLifecycle where TEmbeddedDriver : IEmbeddedDriver, new()
    {
        private readonly IEmbeddedDriverExtractor<TEmbeddedDriver> _extractor;

        protected EmbeddedBrowserLifecycle()
        {
            _extractor = new EmbeddedDriverExtractor<TEmbeddedDriver>(new FileSystem());
        }

        protected override IWebDriver buildDriver()
        {
            if (_extractor.ShouldExtract())
            {
                _extractor.Extract();
            }

            return constructDriver();
        }

        protected abstract IWebDriver constructDriver();
    }
}