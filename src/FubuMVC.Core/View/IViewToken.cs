using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.View
{
    public interface IViewToken
    {
        BehaviorNode ToBehavioralNode();
    }
}