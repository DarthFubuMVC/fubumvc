using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Http.Hosting;
using FubuMVC.Core.Runtime;
using FubuMVC.Katana;

namespace Serenity
{
    public interface ISerenityHosting
    {
        IApplicationUnderTest Start(ApplicationSettings settings, FubuRuntime runtime, IBrowserLifecycle lifecycle);
        void Shutdown();
    }

    public class ExternalHosting : ISerenityHosting
    {
        public IApplicationUnderTest Start(ApplicationSettings settings, FubuRuntime runtime,
            IBrowserLifecycle lifecycle)
        {
            var application = new ApplicationUnderTest(runtime, settings, lifecycle);
            application.Ping();

            return application;
        }

        public void Shutdown()
        {
            // Nothing
        }
    }

    public class KatanaHosting : ISerenityHosting
    {
        private EmbeddedFubuMvcServer _server;

        public IApplicationUnderTest Start(ApplicationSettings settings, FubuRuntime runtime,
            IBrowserLifecycle lifecycle)
        {
            var port = PortFinder.FindPort(settings.Port);
            _server = new EmbeddedFubuMvcServer(runtime, new KatanaHost(), port);

            settings.RootUrl = _server.BaseAddress;

            return new ApplicationUnderTest(runtime, settings, lifecycle);
        }

        public void Shutdown()
        {
            if (_server != null) _server.SafeDispose();

            _server = null;
        }
    }
}