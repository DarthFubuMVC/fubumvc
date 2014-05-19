using FubuCore;
using Owin;

namespace FubuMVC.Core.Http.Owin
{
    [MarkedForTermination]
    public interface IAppBuilderConfiguration
    {
        void Configure(IAppBuilder builder);
    }
}