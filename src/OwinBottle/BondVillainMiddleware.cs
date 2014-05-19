using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Http.Owin.Middleware;

namespace OwinBottle
{
    public class BondVillainMiddleware : IOwinMiddleware
    {
        private readonly Func<IDictionary<string, object>, Task> _inner;

        public BondVillainMiddleware(Func<IDictionary<string, object>, Task> inner)
        {
            _inner = inner;
        }

        public Task Invoke(IDictionary<string, object> environment)
        {
            var writer = new OwinHttpResponse(environment);
            writer.AppendHeader("Slow-Moving", "Laser");

            return _inner(environment);
        }
    }
}