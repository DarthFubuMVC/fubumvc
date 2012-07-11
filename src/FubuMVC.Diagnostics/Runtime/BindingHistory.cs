using FubuCore.Binding.InMemory;
using FubuCore.Binding.Logging;

namespace FubuMVC.Diagnostics.Runtime
{
    public class BindingHistory : IBindingHistory
    {
        private readonly DebugReport _report;

        public BindingHistory(DebugReport report)
        {
            _report = report;
        }

        public void Store(BindingReport report)
        {
            // do nothing
        }
    }
}