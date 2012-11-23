using System;
using System.Collections.Generic;
using Bottles;
using FubuCore.Binding;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Security;

namespace FubuMVC.Core.Runtime
{
    public interface IServiceFactory
    {
        IActionBehavior BuildBehavior(ServiceArguments arguments, Guid behaviorId);
        IEndPointAuthorizor AuthorizorFor(Guid behaviorId);

        T Get<T>();
        IEnumerable<T> GetAll<T>();
    }
}