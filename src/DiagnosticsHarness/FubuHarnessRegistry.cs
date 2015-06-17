using FubuMVC.Core;
using FubuMVC.Katana;

namespace DiagnosticsHarness
{
    public class FubuHarnessRegistry : FubuRegistry
    {
        public FubuHarnessRegistry()
        {
            Import<HarnessRegistry>();
            AlterSettings<KatanaSettings>(x => {
                x.AutoHostingEnabled = true;
            });
        }
    }
}