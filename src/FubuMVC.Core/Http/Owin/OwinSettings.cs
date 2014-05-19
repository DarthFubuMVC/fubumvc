using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuCore.Util;
using FubuMVC.Core.Http.Owin.Middleware;
using FubuMVC.Core.Http.Owin.Middleware.StaticFiles;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Http.Owin
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    [ApplicationLevel]
    public class OwinSettings
    {
        public OwinSettings()
        {
            AddMiddleware<StaticFileMiddleware>();
        }

        /// <summary>
        /// Key value pairs to control or alter the behavior of the underlying host
        /// </summary>
        public readonly IDictionary<string, object> Properties = new Dictionary<string, object>();


        /// <summary>
        /// A list of keys and their associated values that will be injected by the host into each OWIN request environment.
        /// </summary>
        public readonly Cache<string, object> EnvironmentData = new Cache<string, object>();

        // Tested through integration tests
        public AppFunc BuildAppFunc(AppFunc inner, IServiceFactory factory)
        {
            AppFunc func = inner;
            Middleware.Reverse().Each(x => func = x.BuildAppFunc(func, factory));

            return func;
        }


        public readonly MiddlewareChain Middleware = new MiddlewareChain();


        public MiddlewareNode<T> AddMiddleware<T>(BehaviorCategory category = BehaviorCategory.Process) where T : class, IOwinMiddleware
        {
            var node = new MiddlewareNode<T>();
            AddMiddleware(node);

            return node;
        }

        public void AddMiddleware(MiddlewareNode node)
        {
            Middleware.AddToEnd(node);
        }
    }
}