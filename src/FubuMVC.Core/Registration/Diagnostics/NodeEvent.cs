using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Diagnostics
{
    public abstract class NodeEvent
    {
        public object Subject { get; set; }

        public ConfigSource Source { get; set; }

        public BehaviorChain Chain { get; set; }
    }
}