using Bottles;
using Bottles.Diagnostics;

namespace FubuMVC.Katana
{
    public class KatanaHostingDeactivator : IDeactivator
    {
        private readonly KatanaSettings _settings;

        public KatanaHostingDeactivator(KatanaSettings settings)
        {
            _settings = settings;
        }

        public void Deactivate(IPackageLog log)
        {
            if (_settings.EmbeddedServer != null)
            {
                log.Trace("Shutting down the embedded Katana server");
                _settings.EmbeddedServer.Dispose();
            }
        }
    }
}