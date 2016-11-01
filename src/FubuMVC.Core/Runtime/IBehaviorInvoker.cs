using System.Collections.Generic;
using StructureMap.Pipeline;

namespace FubuMVC.Core.Runtime
{
    public interface IBehaviorInvoker
    {
        void Invoke(TypeArguments arguments, IDictionary<string, object> routeValues, IRequestCompletion requestCompletion);
    }
}