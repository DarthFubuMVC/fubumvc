using Owin;

namespace FubuMVC.Core.Http.Owin
{
    public interface IAppBuilderConfiguration
    {
        void Configure(IAppBuilder builder);
    }
}