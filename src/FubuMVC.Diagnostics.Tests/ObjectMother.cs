using Bottles;
using FubuMVC.Core;
using FubuMVC.Core.Registration;

namespace FubuMVC.Diagnostics.Tests
{
    public class ObjectMother
    {
        public static FubuRegistry DiagnosticsRegistry()
        {
            return new FubuDiagnosticsRegistry();
        }
        public static BehaviorGraph DiagnosticsGraph()
        {
            PackageRegistry.LoadPackages(f => { });
            return DiagnosticsRegistry()
                    .BuildGraph();
        }
    }
}