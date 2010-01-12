using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Registration
{
    public interface IBehaviorVisitor
    {
        void VisitBehavior(BehaviorChain chain);
    }
}