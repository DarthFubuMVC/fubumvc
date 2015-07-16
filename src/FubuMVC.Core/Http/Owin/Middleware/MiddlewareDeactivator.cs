using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Diagnostics.Packaging;

namespace FubuMVC.Core.Http.Owin.Middleware
{
    public class MiddlewareDeactivator : IDeactivator
    {
        private readonly OwinSettings _settings;

        public MiddlewareDeactivator(OwinSettings settings)
        {
            _settings = settings;
        }

        public void Deactivate(IActivationLog log)
        {
            _settings.Middleware.OfType<IDisposable>().Each(x => x.Dispose());
        }
    }
}