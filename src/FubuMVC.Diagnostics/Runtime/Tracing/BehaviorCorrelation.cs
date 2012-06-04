using System;

namespace FubuMVC.Diagnostics.Runtime.Tracing
{
    public class BehaviorCorrelation
    {
        public Guid ChainId { get; set; }
        public Guid BehaviorId { get; set; }
    }
}