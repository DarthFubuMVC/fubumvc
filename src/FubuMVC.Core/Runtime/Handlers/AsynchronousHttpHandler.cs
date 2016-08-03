using System.Collections.Generic;
using System.Web.SessionState;
using FubuCore.Binding;
using StructureMap.Pipeline;

namespace FubuMVC.Core.Runtime.Handlers
{
    public class AsynchronousHttpHandler : SessionlessAsynchronousHttpHandler, IRequiresSessionState
    {
        public AsynchronousHttpHandler(IBehaviorInvoker invoker, TypeArguments arguments, IDictionary<string, object> routeData) : base(invoker, arguments, routeData)
        {
        }
    }
}