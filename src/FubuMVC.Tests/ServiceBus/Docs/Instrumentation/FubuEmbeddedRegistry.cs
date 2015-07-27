using FubuMVC.Core;
using FubuMVC.Katana;
using FubuMVC.Tests.ServiceBus.Docs.GettingStarted;

namespace FubuMVC.Tests.ServiceBus.Docs.Instrumentation
{
    // SAMPLE: FubuEmbeddedRegistrySample
    public class FubuEmbeddedRegistry : FubuRegistry
    {
        public FubuEmbeddedRegistry()
        {
            Import<GettingStartedTransportRegistry>();
            AlterSettings<KatanaSettings>(x =>
            {
                x.Port = 5500;
                x.AutoHostingEnabled = true;
            });
        }
    }
    // ENDSAMPLE
}