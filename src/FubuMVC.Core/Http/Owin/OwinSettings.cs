using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuCore.Descriptions;
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
    public class OwinSettings : DescribesItself
    {
        public OwinSettings()
        {
            AddMiddleware<StaticFileMiddleware>();
            HtmlHeadInjectionMiddleware.ApplyInjection(this);
        }

        public void Describe(Description description)
        {
            description.Title = "OWIN Settings";
            description.ShortDescription =
                "Governs the attachment and ordering of OWIN middleware plus OWIN host properties";
            Properties.Each(x => description.Properties[x.Key] = x.Value.ToString());

            EnvironmentData.Each((key, value) => description.Properties[key] = value.ToString());

            var middleware = new BulletList();
            middleware.Name = "Middleware";
            middleware.Label = "Middleware";

            Middleware.Each(x => middleware.Children.Add(x.ToDescription()));

            description.BulletLists.Add(middleware);
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

        public AnonymousMiddleware AddMiddleware(AppFunc appFunc)
        {
            Func<AppFunc, AppFunc> appFuncSource = inner => {
                return dict => {
                    return appFunc(dict).ContinueWith(t => inner.Invoke(dict));
                };
            };

            return AddMiddleware(appFuncSource);
        }

        public AnonymousMiddleware AddMiddleware(Func<AppFunc, AppFunc> appFuncSource)
        {
            var node = new AnonymousMiddleware(appFuncSource);
            AddMiddleware(node);

            return node;
        }


    }

    public class AnonymousMiddleware : MiddlewareNode
    {
        private readonly Func<AppFunc, AppFunc> _builder;
        private string _description = "Anonymous Middleware";

        public AnonymousMiddleware(Func<AppFunc, AppFunc> builder)
        {
            _builder = builder;
        }

        public override AppFunc BuildAppFunc(AppFunc inner, IServiceFactory factory)
        {
            return _builder(inner);
        }

        public string Description()
        {
            return _description;
        }

        public void Description(string description)
        {
            _description = description;
        }

        public override Description ToDescription()
        {
            return new Description
            {
                Title = _description,
                ShortDescription = "Category: " + Category()
            };
        }
    }
}