using System;
using Bottles;
using Bottles.Diagnostics;

namespace FubuMVC.Nowin
{
    public class KatanaHostingDeactivator : IDeactivator
    {
        private readonly NowinSettings _settings;

        public KatanaHostingDeactivator(NowinSettings settings)
        {
            _settings = settings;
        }

        public void Deactivate(IPackageLog log)
        {
            if (_settings.EmbeddedServer != null)
            {
                Console.WriteLine("Shutting down the embedded Nowin server");
                log.Trace("Shutting down the embedded Nowin server");
                _settings.EmbeddedServer.Dispose();
            }
        }
    }
}
