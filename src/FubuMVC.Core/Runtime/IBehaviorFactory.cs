using System;
using FubuCore.Binding;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Security;

namespace FubuMVC.Core.Runtime
{
    public interface IBehaviorFactory
    {
        IActionBehavior BuildBehavior(ServiceArguments arguments, Guid behaviorId);
        IEndPointAuthorizor AuthorizorFor(Guid behaviorId);
    }
}