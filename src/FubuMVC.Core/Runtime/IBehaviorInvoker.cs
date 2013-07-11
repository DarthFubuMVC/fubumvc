using System.Collections.Generic;
using FubuCore.Binding;

namespace FubuMVC.Core.Runtime
{
    public interface IBehaviorInvoker
    {
        void Invoke(ServiceArguments arguments, IDictionary<string, object> routeValues, IRequestCompletion requestCompletion);
    }
}