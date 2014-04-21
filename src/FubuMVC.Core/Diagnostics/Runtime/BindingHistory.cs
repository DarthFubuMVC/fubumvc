using System.ComponentModel;
using FubuCore.Binding.InMemory;
using FubuCore.Binding.Logging;
using FubuCore.Logging;

namespace FubuMVC.Core.Diagnostics.Runtime
{
    public class BindingHistory : IBindingHistory
    {
        private readonly ILogger _logger;

        public BindingHistory(ILogger logger)
        {
            _logger = logger;
        }

        public void Store(BindingReport report)
        {
            _logger.DebugMessage(() => new ModelBindingLog{Report = report});
        }
    }

    [Description("Model Binding Log")]
    public class ModelBindingLog : LogRecord
    {
        public BindingReport Report { get; set; }
    }
}