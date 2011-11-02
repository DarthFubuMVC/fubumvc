using FubuCore.Binding;

namespace FubuMVC.Core.Runtime
{
    public interface IBehaviorInvocationFilter
    {
        DoNext Filter(ServiceArguments arguments);
    }
}