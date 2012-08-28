using System.Collections.Generic;
using System.Web.SessionState;
using FubuCore.Binding;

namespace FubuMVC.Core.Runtime.Handlers
{
    public class AsynchronousHttpHandler : SessionlessAsynchronousHttpHandler, IRequiresSessionState
    {
        public AsynchronousHttpHandler(IBehaviorInvoker invoker, ServiceArguments arguments, IDictionary<string, object> routeData) : base(invoker, arguments, routeData)
        {
        }
    }
}