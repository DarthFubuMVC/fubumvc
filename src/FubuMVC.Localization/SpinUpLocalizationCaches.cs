using System.Collections.Generic;
using Bottles;
using Bottles.Diagnostics;
using FubuLocalization.Basic;

namespace FubuMVC.Localization
{
    public class SpinUpLocalizationCaches : IActivator
    {
        private readonly ILocalizationProviderFactory _factory;

        public SpinUpLocalizationCaches(ILocalizationProviderFactory factory)
        {
            _factory = factory;
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            _factory.LoadAll(text => log.Trace(text));
            _factory.ApplyToLocalizationManager();
        }
    }
}