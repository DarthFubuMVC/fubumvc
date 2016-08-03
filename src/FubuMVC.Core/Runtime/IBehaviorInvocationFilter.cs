using FubuCore.Binding;
using StructureMap.Pipeline;

namespace FubuMVC.Core.Runtime
{
    public interface IBehaviorInvocationFilter
    {
        DoNext Filter(TypeArguments arguments);
    }
}