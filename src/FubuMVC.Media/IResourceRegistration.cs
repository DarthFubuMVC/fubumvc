using FubuMVC.Core.Registration;

namespace FubuMVC.Media
{
    public interface IResourceRegistration
    {
        void Modify(ConnegGraph graph, BehaviorGraph behaviorGraph);
    }
}