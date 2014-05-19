using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FubuMVC.Core;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Http.Owin.Middleware;
using FubuMVC.Katana;
using FubuMVC.StructureMap;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Owin
{
    [TestFixture]
    public class using_custom_middleware
    {
        private EmbeddedFubuMvcServer serverFor(Action<OwinSettings> action)
        {
            var registry = new FubuRegistry();
            registry.AlterSettings(action);
            return FubuApplication.For(registry).StructureMap().RunEmbedded(port:0);
        }

        [Test]
        public void can_add_a_single_OWIN_middleware()
        {
            using (var server = serverFor(x => {
                x.AddMiddleware<JamesBondMiddleware>();
            }))
            {
                server.Endpoints.Get<MiddleWareInterceptedEndpoint>(x => x.get_middleware_result())
                    .ShouldHaveHeaderValue("James", "Bond");
            }
        }
    }


    public class MiddleWareInterceptedEndpoint
    {
        public string get_middleware_result()
        {
            return "I'm okay";
        }
    }
    

    public class JamesBondMiddleware : IOwinMiddleware
    {
        private readonly Func<IDictionary<string, object>, Task> _inner;

        public JamesBondMiddleware(Func<IDictionary<string, object>, Task> inner)
        {
            _inner = inner;
        }

        public Task Invoke(IDictionary<string, object> environment)
        {
            var writer = new OwinHttpResponse(environment);
            writer.AppendHeader("James", "Bond");

            return _inner(environment);
        }
    }
}