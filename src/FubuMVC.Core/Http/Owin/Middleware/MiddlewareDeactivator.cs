using System;
using System.Collections.Generic;
using System.Linq;
using Bottles;
using Bottles.Diagnostics;

namespace FubuMVC.Core.Http.Owin.Middleware
{
    public class MiddlewareDeactivator : IDeactivator
    {
        private readonly OwinSettings _settings;

        public MiddlewareDeactivator(OwinSettings settings)
        {
            _settings = settings;
        }

        public void Deactivate(IPackageLog log)
        {
            _settings.Middleware.OfType<IDisposable>().Each(x => x.Dispose());
        }
    }
}