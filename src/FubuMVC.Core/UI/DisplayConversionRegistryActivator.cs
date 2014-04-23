using System.Collections.Generic;
using Bottles;
using Bottles.Diagnostics;
using FubuCore.Formatting;

namespace FubuMVC.Core.UI
{
    // TODO -- get rid of this maybe?  Bring into BehaviorGraphBuilder?
    public class DisplayConversionRegistryActivator : IActivator
    {
        private readonly IEnumerable<DisplayConversionRegistry> _registries;
        private readonly Stringifier _stringifier;

        public DisplayConversionRegistryActivator(IEnumerable<DisplayConversionRegistry> registries, Stringifier stringifier)
        {
            _registries = registries;
            _stringifier = stringifier;
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            _registries.Each(r =>
            {
                log.Trace("Adding " + r);
                r.Configure(_stringifier);
            });
        }
    }
}