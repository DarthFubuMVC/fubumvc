using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using FubuCore.Util;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Registration.ObjectGraph;
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

            Middleware.OfType<IAppBuilderConfiguration>().Each(x => x.Configure(builder));
        }

        public readonly MiddlewareChain Middleware = new MiddlewareChain();

        public void AddMiddleware<T>(params object[] args) where T : class
        {
            Middleware.AddToEnd(new Middleware<T>());
        }
    }

    public interface IAppBuilderConfiguration
    {
        void Configure(IAppBuilder builder);
    }

    public class MiddlewareChain : Chain<MiddlewareNode, MiddlewareChain>
    {
        
    }

    public abstract class MiddlewareNode : Node<MiddlewareNode, MiddlewareChain>, IAppBuilderConfiguration
    {
        void IAppBuilderConfiguration.Configure(IAppBuilder builder)
        {
            configure(builder);
        }

        protected abstract void configure(IAppBuilder builder);

        /// <summary>
        /// Can be used to opt into ordering rules
        /// </summary>
        public BehaviorCategory Category = BehaviorCategory.Process;
    }

    public class AppFuncMiddleware : MiddlewareNode
    {
        private readonly string _description;
        private readonly object _middleware;

        public AppFuncMiddleware(string description, AppFunc func)
        {
            _description = description;
            _middleware = func;
        }

        public AppFuncMiddleware(string description, Func<IDictionary<string, object>, AppFunc, Task> wrappedFunc)
        {
            _description = description;
            _middleware = wrappedFunc;
        }

        protected override void configure(IAppBuilder builder)
        {
            builder.Use(_middleware);
        }

        public string Description
        {
            get { return _description; }
        }
    }

    public class Middleware<T> : MiddlewareNode where T : class
    {
        private readonly object[] _args;

        public Middleware(params object[] args)
        {
            _args = args;
            // TODO -- verify the ctor signature
        }

        protected override void configure(IAppBuilder builder)
        {
            builder.Use(typeof (T), _args);
        }
    }

    public class Middleware<T, TOptions> : MiddlewareNode where T : class where TOptions : class, new()
    {
        private readonly TOptions _options;


        public Middleware()
        {
            _options = new TOptions();

            // TODO -- verify the ctor signature
        }

        public TOptions Options
        {
            get { return _options; }
        }

        protected override void configure(IAppBuilder builder)
        {
            builder.Use(typeof (T), _options);
        }
    }
}