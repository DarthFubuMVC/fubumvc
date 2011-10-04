using FubuMVC.Core.Registration;

namespace FubuMVC.Core.Resources
{
    public interface IResourceRegistration
    {
        void Modify(ConnegGraph graph, BehaviorGraph behaviorGraph);
    }
}