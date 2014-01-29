using Owin;

namespace FubuMVC.OwinHost
{
    public interface IAppBuilderConfiguration
    {
        void Configure(IAppBuilder builder);
    }
}