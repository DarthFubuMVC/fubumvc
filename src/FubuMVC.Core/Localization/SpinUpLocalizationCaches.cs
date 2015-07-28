using FubuLocalization.Basic;
using FubuMVC.Core.Diagnostics.Packaging;

namespace FubuMVC.Core.Localization
{
    public class SpinUpLocalizationCaches : IActivator
    {
        private readonly ILocalizationProviderFactory _factory;

        public SpinUpLocalizationCaches(ILocalizationProviderFactory factory)
        {
            _factory = factory;
        }

        public void Activate(IActivationLog log, IPerfTimer timer)
        {
            _factory.LoadAll(text => log.Trace(text));
            _factory.ApplyToLocalizationManager();
        }
    }
}