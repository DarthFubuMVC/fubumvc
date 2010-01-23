using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Registration
{
    public interface IUrlRegistrationConvention
    {
        void Configure(BehaviorGraph graph, IUrlRegistration registration);
    }
}