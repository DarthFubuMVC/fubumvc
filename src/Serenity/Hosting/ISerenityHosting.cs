using FubuMVC.Core;

namespace Serenity.Hosting
{
    public interface ISerenityHosting
    {
        IApplicationUnderTest Start(ApplicationSettings settings, FubuRuntime runtime, IBrowserLifecycle lifecycle);
        void Shutdown();
    }
}
