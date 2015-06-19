using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Katana;

namespace Serenity.Hosting
{
    public class KatanaHosting : ISerenityHosting
    {
        private EmbeddedFubuMvcServer _server;

        public IApplicationUnderTest Start(ApplicationSettings settings, FubuRuntime runtime, IBrowserLifecycle lifecycle)
        {
            var port = PortFinder.FindPort(settings.Port);
            _server = new EmbeddedFubuMvcServer(runtime, settings.PhysicalPath, port);

            settings.RootUrl = _server.BaseAddress;

            var graph = runtime.Factory.Get<BehaviorGraph>();

            return new ApplicationUnderTest(runtime, settings, lifecycle);
        }

        public void Shutdown()
        {
            if (_server != null) _server.SafeDispose();

            _server = null;
        }
    }
}
