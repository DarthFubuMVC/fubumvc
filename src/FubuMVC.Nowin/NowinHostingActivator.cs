using System;
using System.Collections.Generic;
using Bottles;
using Bottles.Diagnostics;
using FubuMVC.Core;

namespace FubuMVC.Nowin
{
    public class NowinHostingActivator : IActivator
    {
        private readonly FubuRuntime _runtime;
        private readonly NowinSettings _settings;

        public NowinHostingActivator(FubuRuntime runtime, NowinSettings settings)
        {
            _runtime = runtime;
            _settings = settings;
        }

        public void Activate(IPackageLog log)
        {
            if (!_settings.AutoHostingEnabled)
            {
                log.Trace("Embedded Nowin hosting is not enabled");
                return;
            }

            Console.WriteLine("Starting Nowin hosting at port " + _settings.Port);
            log.Trace("Starting Nowin hosting at port " + _settings.Port);

            _settings.EmbeddedServer = new EmbeddedFubuMvcServer(_runtime, port:_settings.Port);
        }
    }
}
