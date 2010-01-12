using System;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration.Conventions
{
    [Obsolete("Is this necessary?")]
    public interface IChainConvention
    {
        DoNext Apply(BehaviorGraph graph, BehaviorChain chain);
    }
}