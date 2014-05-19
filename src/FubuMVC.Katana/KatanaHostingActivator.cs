using System;
using System.Collections.Generic;
using Bottles;
using Bottles.Diagnostics;
using FubuMVC.Core;

namespace FubuMVC.Katana
{
    public class KatanaHostingActivator : IActivator
    {
        private readonly FubuRuntime _runtime;
        private readonly KatanaSettings _settings;

        public KatanaHostingActivator(FubuRuntime runtime, KatanaSettings settings)
        {
            _runtime = runtime;
            _settings = settings;
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            if (!_settings.AutoHostingEnabled)
            {
                log.Trace("Embedded Katana hosting is not enabled");
                return;
            }

            Console.WriteLine("Starting Katana hosting at port " + _settings.Port);
            log.Trace("Starting Katana hosting at port " + _settings.Port);

            _settings.EmbeddedServer = new EmbeddedFubuMvcServer(_runtime, port:_settings.Port);
        }
    }
}