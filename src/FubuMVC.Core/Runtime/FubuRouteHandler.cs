using System;
using System.Web;
using System.Web.Routing;
using System.Web.SessionState;
using FubuCore.Binding;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Http.AspNet;

namespace FubuMVC.Core.Runtime
{
    public class FubuRouteHandler : IRouteHandler
    {
        private readonly Guid _behaviorId;
        private readonly IBehaviorFactory _factory;

        public FubuRouteHandler(IBehaviorFactory factory, Guid behaviorId)
        {
            _factory = factory;
            _behaviorId = behaviorId;
        }

        public Guid BehaviorId
        {
            get { return _behaviorId; }
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var arguments = new AspNetServiceArguments(requestContext);
            IActionBehavior behavior = GetBehavior(arguments);

            return new FubuHttpHandler(behavior);
        }

        public IActionBehavior GetBehavior(ServiceArguments arguments)
        {
            return _factory.BuildBehavior(arguments, _behaviorId);
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