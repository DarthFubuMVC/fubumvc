using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Descriptions;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Http.Owin.Middleware
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public abstract class MiddlewareNode : Node<MiddlewareNode, MiddlewareChain>
    {
        private BehaviorCategory _category = BehaviorCategory.Process;


        public MiddlewareNode Category(BehaviorCategory category)
        {
            _category = category;
            return this;
        }

        public BehaviorCategory Category()
        {
            return _category;
        }

        public abstract AppFunc BuildAppFunc(AppFunc inner, IServiceFactory factory);
        public abstract Description ToDescription();
    }

    public class MiddlewareNode<T> : MiddlewareNode, IDisposable where T : class, IOwinMiddleware
    {
        // TODO -- add other service arguments
        public readonly ServiceArguments Arguments = new ServiceArguments();
        private T _middleware;

        // Tested through integration tests
        public override AppFunc BuildAppFunc(AppFunc inner, IServiceFactory factory)
        {
            Arguments.With(inner);
            _middleware = factory.Build<T>(Arguments);
            

            return _middleware.Invoke;
        }

        public T Middleware
        {
            get { return _middleware; }
        }

        public override Description ToDescription()
        {
            if (_middleware == null)
            {
                return new Description
                {
                    Title = typeof(T).FullName
                };
            }

            return Description.For(_middleware);
        }

        public void Dispose()
        {
            var disposable = _middleware as IDisposable;
            disposable.CallIfNotNull(x => x.Dispose());
        }
    }
}