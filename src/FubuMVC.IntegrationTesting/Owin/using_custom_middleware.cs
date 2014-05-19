using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FubuMVC.Core;
using FubuMVC.Core.Http.Owin;
using FubuMVC.Core.Http.Owin.Middleware;
using FubuMVC.Katana;
using FubuMVC.StructureMap;
using FubuTestingSupport;
using NUnit.Framework;

namespace FubuMVC.IntegrationTesting.Owin
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

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

        [Test]
        public void use_anonymous_AppFunc_as_middleware()
        {
            AppFunc sillyHeader = dict => {
                return Task.Factory.StartNew(() => {
                    new OwinHttpResponse(dict).AppendHeader("silly", "string");
                });
            };

            using (var server = serverFor(x => {
                x.AddMiddleware(sillyHeader);
            }))
            {
                server.Endpoints.Get<MiddleWareInterceptedEndpoint>(x => x.get_middleware_result())
                    .ShouldHaveHeaderValue("silly", "string")
                    .ReadAsText().ShouldContain("I'm okay");
            }
        }

        [Test]
        public void use_anonymous_wrapping_Func_AppFunc_AppFunc_as_middleware()
        {
            Func<AppFunc, AppFunc> middleware = inner => {
                return dict => {
                    var response = new OwinHttpResponse(dict);

                    response.Write("1-");
                    response.Flush();
                    return inner(dict).ContinueWith(t => {
                        response.Write("-2");
                        response.Flush();
                    });
                };
            };

            using (var server = serverFor(x =>
            {
                x.AddMiddleware(middleware);
            }))
            {
                server.Endpoints.Get<MiddleWareInterceptedEndpoint>(x => x.get_middleware_result())
                    .ReadAsText().ShouldContain("1-I'm okay-2");
            }


        }

        [Test]
        public void disposes_any_disposable_middleware_on_shutdown()
        {
            MiddlewareNode<SpecialDisposableMiddleware> node = null;

            using (var server = serverFor(x =>
            {
                node = x.AddMiddleware<SpecialDisposableMiddleware>();
            }))
            {
                server.Endpoints.Get<MiddleWareInterceptedEndpoint>(x => x.get_middleware_result())
                    .ReadAsText().ShouldContain("I'm okay");
            }

            node.Middleware.IWasDisposed.ShouldBeTrue();
        }


    }

    public class SpecialDisposableMiddleware : IOwinMiddleware, IDisposable
    {
        private readonly AppFunc _inner;
        public bool IWasDisposed;

        public SpecialDisposableMiddleware(AppFunc inner)
        {
            _inner = inner;
        }

        public Task Invoke(IDictionary<string, object> environment)
        {
            return _inner(environment);
        }

        public void Dispose()
        {
            IWasDisposed = true;
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