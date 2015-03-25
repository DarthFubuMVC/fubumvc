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
            var settings = StoryTellerEnvironment.Get<SerenityEnvironment>();
            _extractor = new EmbeddedDriverExtractor<TEmbeddedDriver>(settings, new FileSystem());
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