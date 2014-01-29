using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Katana;
using FubuMVC.StructureMap;
using NUnit.Framework;
using Owin;

namespace FubuMVC.OwinHost.Testing
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    [TestFixture]
    public class using_custom_middleware
    {
        private EmbeddedFubuMvcServer serverFor(Action<OwinSettings> action)
        {
            var registry = new FubuRegistry();
            registry.AlterSettings(action);
            return FubuApplication.For(registry).StructureMap().RunEmbedded(autoFindPort:true);
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
    

    public class JamesBondMiddleware
    {
        private readonly AppFunc _inner;

        public JamesBondMiddleware(AppFunc inner)
        {
            _inner = inner;
        }

        public Task Invoke(IDictionary<string, object> environment)
        {
            var writer = new OwinHttpWriter(environment);
            writer.AppendHeader("James", "Bond");

            return _inner(environment);
        }
    }
}