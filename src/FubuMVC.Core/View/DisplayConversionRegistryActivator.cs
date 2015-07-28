using System.Collections.Generic;
using FubuCore.Formatting;
using FubuMVC.Core.Diagnostics.Packaging;

namespace FubuMVC.Core.View
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

        public void Activate(IActivationLog log, IPerfTimer timer)
        {
            _registries.Each(r =>
            {
                log.Trace("Adding " + r);
                r.Configure(_stringifier);
            });
        }
    }
}