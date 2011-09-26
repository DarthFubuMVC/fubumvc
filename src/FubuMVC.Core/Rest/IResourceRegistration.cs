using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Rest
{
    public interface IResourceRegistration
    {
        void Modify(ConnegGraph graph, BehaviorGraph behaviorGraph);
    }
}