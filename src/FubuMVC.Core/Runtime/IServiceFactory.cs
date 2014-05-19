using System;
using System.Collections.Generic;
using FubuCore.Binding;
using FubuMVC.Core.Behaviors;

namespace FubuMVC.Core.Runtime
{
    public interface IServiceFactory : IDisposable
    {
        IActionBehavior BuildBehavior(ServiceArguments arguments, Guid behaviorId);

        T Build<T>(ServiceArguments arguments);

        T Get<T>();
        IEnumerable<T> GetAll<T>();
    }
}