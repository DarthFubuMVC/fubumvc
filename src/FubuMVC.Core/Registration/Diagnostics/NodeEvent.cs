using FubuMVC.Core.Registration.ObjectGraph;

namespace FubuMVC.Core.Registration.Diagnostics
{
    public abstract class NodeEvent
    {
        public object Subject { get; set; }

        public ConfigSource Source { get; set; }
    }
}