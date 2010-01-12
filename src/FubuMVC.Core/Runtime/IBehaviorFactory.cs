using System;
using FubuMVC.Core.Behaviors;

namespace FubuMVC.Core.Runtime
{
    public interface IBehaviorFactory
    {
        IActionBehavior BuildBehavior(ServiceArguments arguments, Guid behaviorId);
    }
}