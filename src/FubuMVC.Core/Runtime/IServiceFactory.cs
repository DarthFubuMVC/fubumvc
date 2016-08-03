using System;
using System.Collections.Generic;
using FubuMVC.Core.Behaviors;
using StructureMap.Pipeline;

namespace FubuMVC.Core.Runtime
{
    public interface IServiceFactory : IDisposable
    {
        IActionBehavior BuildBehavior(TypeArguments arguments, Guid behaviorId);

        T Build<T>(TypeArguments arguments);

        T Get<T>();
        IEnumerable<T> GetAll<T>();
    }
}