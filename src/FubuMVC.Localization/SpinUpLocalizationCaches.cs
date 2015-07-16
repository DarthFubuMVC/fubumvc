using System.Collections.Generic;
using FubuLocalization.Basic;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics.Packaging;

namespace FubuMVC.Localization
{
    public class SpinUpLocalizationCaches : IActivator
    {
        private readonly ILocalizationProviderFactory _factory;

        public SpinUpLocalizationCaches(ILocalizationProviderFactory factory)
        {
            _factory = factory;
        }

        public void Activate(IPackageLog log)
        {
            _factory.LoadAll(text => log.Trace(text));
            _factory.ApplyToLocalizationManager();
        }
    }
}