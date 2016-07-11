using System.Collections.Generic;
using System.Threading.Tasks;
using FubuCore.Binding;
using StructureMap.Pipeline;

namespace FubuMVC.Core.Runtime
{
    public interface IBehaviorInvoker
    {
        void Invoke(TypeArguments arguments, IDictionary<string, object> routeValues);
    }
}