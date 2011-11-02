using System;
using System.Collections.Generic;
using FubuCore.Binding;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Runtime
{
    public interface IBehaviorFactory
    {
        IActionBehavior BuildBehavior(ServiceArguments arguments, Guid behaviorId);
    }


}