using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using System.Web.SessionState;
using FubuCore.Binding;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Http.AspNet;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Runtime
{
    public class FubuRouteHandler : IRouteHandler
    {
        private readonly IBehaviorFactory _factory;
        private readonly BehaviorChain _chain;

        public FubuRouteHandler(IBehaviorFactory factory, BehaviorChain chain)
        {
            _factory = factory;
            _chain = chain;
        }

        public BehaviorChain Chain
        {
            get { return _chain; }
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var arguments = new AspNetServiceArguments(requestContext);
            IActionBehavior behavior = GetBehavior(arguments, requestContext.RouteData.Values);

            return new FubuHttpHandler(behavior);
        }

        public IActionBehavior GetBehavior(ServiceArguments arguments, IDictionary<string, object> values)
        {
            return _factory.BuildBehavior(arguments, _chain, values);
        }

        #region Nested type: FubuHttpHandler

        public class FubuHttpHandler : IHttpHandler, IRequiresSessionState
        {
            private readonly IActionBehavior _behavior;

            public FubuHttpHandler(IActionBehavior behavior)
            {
                _behavior = behavior;
            }

            public void ProcessRequest(HttpContext context)
            {
                _behavior.Invoke();
            }

            public bool IsReusable { get { return false; } }
        }

        #endregion
    }
}