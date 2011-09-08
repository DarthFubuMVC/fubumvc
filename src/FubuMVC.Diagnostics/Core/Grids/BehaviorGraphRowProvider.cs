using System.Collections.Generic;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Diagnostics.Core.Grids
{
    public class BehaviorGraphRowProvider : IGridRowProvider<BehaviorGraph, BehaviorChain>
    {
        public IEnumerable<BehaviorChain> RowsFor(BehaviorGraph target)
        {
            return target.Behaviors;
        }
    }
}