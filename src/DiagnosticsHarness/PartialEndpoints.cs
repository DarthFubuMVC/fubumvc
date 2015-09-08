using FubuMVC.Core;

namespace DiagnosticsHarness
{
    public class PartialEndpoints
    {
        [FubuPartial]
        public string Red()
        {
            return "Red";
        }

        [FubuPartial]
        public string Green()
        {
            return "Green";
        }
    }
}