using System;
using System.Collections.Generic;
using System.Web.Routing;
using Bottles;
using Bottles.Diagnostics;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.Urls;

namespace FubuMVC.Katana
{
    public class KatanaHostingActivator : IActivator
    {
        private readonly KatanaSettings _settings;
        private readonly IList<RouteBase> _routes;
        private readonly IUrlRegistry _urls;
        private readonly IServiceLocator _services;

        public KatanaHostingActivator(KatanaSettings settings, FubuRouteTable routes, IUrlRegistry urls, IServiceLocator services)
        {
            _settings = settings;
            _routes = routes.Routes;
            _urls = urls;
            _services = services;
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

            _settings.EmbeddedServer = new EmbeddedFubuMvcServer(_settings, _urls, _services, _routes);
        }
    }
}