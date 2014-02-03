using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime.Files;
using FubuMVC.Core.Security;
using FubuMVC.OwinHost.Middleware;
using FubuMVC.OwinHost.Middleware.StaticFiles;
using Owin;

namespace FubuMVC.OwinHost
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    [ApplicationLevel]
    public class OwinSettings : IAppBuilderConfiguration
    {
        /// <summary>
        /// Key value pairs to control or alter the behavior of the underlying host
        /// </summary>
        public readonly IDictionary<string, object> Properties = new Dictionary<string, object>();


        /// <summary>
        /// A list of keys and their associated values that will be injected by the host into each OWIN request environment.
        /// </summary>
        public readonly Cache<string, object> EnvironmentData = new Cache<string, object>();

        void IAppBuilderConfiguration.Configure(IAppBuilder builder)
        {
            EnvironmentData.Each((key, value) => {
                if (builder.Properties.ContainsKey(key))
                {
                    builder.Properties[key] = value;
                }
                else
                {
                    builder.Properties.Add(key, value);
                }
            });

            

            Middleware.OfType<IAppBuilderConfiguration>().ToArray().Each(x => x.Configure(builder));
        }

        public readonly MiddlewareChain Middleware = new MiddlewareChain();

        public MiddlewareNode AddMiddleware<T>(params object[] args) where T : class
        {
            var description = "{0} - {1}".ToFormat(typeof (T).FullName,
                args.Select(x => (x ?? string.Empty).ToString()).Join(", "));
            var node = new MiddlewareNode(x => x.Use(typeof (T), args)).Description(description);
            Middleware.AddToEnd(node);

            return node;
        }

        public void AddMiddleware(MiddlewareNode node)
        {
            Middleware.AddToEnd(node);
        }

        public readonly IList<IStaticFileRule> StaticFileRules
            = new List<IStaticFileRule> {new AssetStaticFileRule(), new DenyConfigRule()};

        public AuthorizationRight DetermineStaticFileRights(IFubuFile file)
        {
            return AuthorizationRight.Combine(StaticFileRules.Select(x => x.IsAllowed(file)));
        }
    }
}