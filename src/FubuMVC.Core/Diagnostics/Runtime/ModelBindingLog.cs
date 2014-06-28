using System.ComponentModel;
using FubuCore.Binding.Logging;
using FubuCore.Logging;

namespace FubuMVC.Core.Diagnostics.Runtime
{
    [Description("Model Binding Log")]
    public class ModelBindingLog : LogRecord
    {
        public BindingReport Report { get; set; }
    }
}