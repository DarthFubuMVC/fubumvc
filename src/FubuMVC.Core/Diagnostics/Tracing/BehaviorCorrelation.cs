using System;

namespace FubuMVC.Core.Diagnostics.Tracing
{
    public class BehaviorCorrelation
    {
        public Guid ChainId { get; set; }
        public Guid BehaviorId { get; set; }
    }
}