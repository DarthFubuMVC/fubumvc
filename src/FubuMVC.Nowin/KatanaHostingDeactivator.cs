using System;
using FubuMVC.Core;
using FubuMVC.Core.Diagnostics.Packaging;

namespace FubuMVC.Nowin
{
    public class KatanaHostingDeactivator : IDeactivator
    {
        private readonly NowinSettings _settings;

        public KatanaHostingDeactivator(NowinSettings settings)
        {
            _settings = settings;
        }

        public void Deactivate(IActivationLog log)
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
