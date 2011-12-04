using System;
using System.Collections.Generic;
using FubuCore.Binding;

namespace FubuMVC.Core.Runtime
{
    public interface IBehaviorInvoker
    {
        void Invoke(ServiceArguments arguments, IDictionary<string, object> routeValues);
        void Invoke(ServiceArguments arguments, IDictionary<string, object> routeValues, Action onComplete);
    }
}