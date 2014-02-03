using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FubuMVC.OwinHost;

namespace OwinBottle
{
    public class BondVillainMiddleware
    {
        private readonly Func<IDictionary<string, object>, Task> _inner;

        public BondVillainMiddleware(Func<IDictionary<string, object>, Task> inner)
        {
            _inner = inner;
        }

        public Task Invoke(IDictionary<string, object> environment)
        {
            var writer = new OwinHttpWriter(environment);
            writer.AppendHeader("Slow-Moving", "Laser");

            return _inner(environment);
        }
    }
}