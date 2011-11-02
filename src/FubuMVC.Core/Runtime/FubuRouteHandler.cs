using System;
using System.Collections.Generic;
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
        private readonly IBehaviorInvoker _invoker;

        public FubuRouteHandler(IBehaviorInvoker invoker)
        {
            _invoker = invoker;
        }


        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var arguments = new AspNetServiceArguments(requestContext);
            return new FubuHttpHandler(_invoker, arguments, requestContext.RouteData.Values);
        }

        public IBehaviorInvoker Invoker
        {
            get { return _invoker; }
        }

        #region Nested type: FubuHttpHandler

        public class FubuHttpHandler : IHttpHandler, IRequiresSessionState
        {
            private readonly IBehaviorInvoker _invoker;
            private readonly ServiceArguments _arguments;
            private readonly IDictionary<string, object> _routeData;

            public FubuHttpHandler(IBehaviorInvoker invoker, ServiceArguments arguments, IDictionary<string, object> routeData)
            {
                _invoker = invoker;
                _arguments = arguments;
                _routeData = routeData;
            }

            public void ProcessRequest(HttpContext context)
            {
                _invoker.Invoke(_arguments, _routeData);
            }

            public bool IsReusable { get { return false; } }
        }

        #endregion
    }
}