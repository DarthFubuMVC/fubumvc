using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Runtime
{
    public interface IPartialFactory
    {
        IActionBehavior BuildBehavior(BehaviorChain chain);
        IActionBehavior BuildPartial(BehaviorChain chain);
        
    }
}