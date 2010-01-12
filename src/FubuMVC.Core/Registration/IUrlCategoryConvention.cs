using FubuMVC.Core.Urls;

namespace FubuMVC.Core.Registration
{
    public interface IUrlCategoryConvention
    {
        void Configure(BehaviorGraph graph, IUrlRegistration registration);
    }
}