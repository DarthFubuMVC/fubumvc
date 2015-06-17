using FubuMVC.Core;
using FubuMVC.Katana;
using FubuTransportation.Testing.Docs.GettingStarted;

namespace FubuTransportation.Testing.Docs.Instrumentation
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