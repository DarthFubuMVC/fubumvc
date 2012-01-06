using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using System.Web.SessionState;
using FubuCore.Binding;
using FubuMVC.Core.Http.AspNet;

namespace FubuMVC.Core.Runtime
{
    public class FubuAsyncRouteHandler : IFubuRouteHandler
    {
        private readonly IBehaviorInvoker _invoker;

        public FubuAsyncRouteHandler(IBehaviorInvoker invoker)
        {
            _invoker = invoker;
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var arguments = new AspNetServiceArguments(requestContext);
            return new FubuAsyncHttpHandler(_invoker, arguments, requestContext.RouteData.Values);
        }

        public IBehaviorInvoker Invoker
        {
            get { return _invoker; }
        }

        public class FubuAsyncHttpHandler : IHttpAsyncHandler, IRequiresSessionState
        {
            private readonly IBehaviorInvoker _invoker;
            private readonly ServiceArguments _arguments;
            private readonly IDictionary<string, object> _routeData;

            public FubuAsyncHttpHandler(IBehaviorInvoker invoker, ServiceArguments arguments, IDictionary<string, object> routeData)
            {
                _invoker = invoker;
                _arguments = arguments;
                _routeData = routeData;
            }

            public void ProcessRequest(HttpContext context)
            {
                throw new InvalidOperationException("Synchronous requests are not supported with this handler");
            }

            public bool IsReusable
            {
                get { return false; }
            }

            public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
            {
                var task = Task.Factory.StartNew(() => _invoker.Invoke(_arguments, _routeData));
                task.ContinueWith(x => cb(task));
                return task;
            }

            public void EndProcessRequest(IAsyncResult result)
            {
                var task = (Task) result;
                //So any unobserved exceptions or results don't kill the process.
                task.Wait();
            }
        }
    }
}