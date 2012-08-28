using System.Collections.Generic;
using System.Web.SessionState;
using FubuCore.Binding;

namespace FubuMVC.Core.Runtime.Handlers
{
    public class SynchronousFubuHttpHandler : SessionLessFubuHttpHandler, IRequiresSessionState
    {
        public SynchronousFubuHttpHandler(IBehaviorInvoker invoker, ServiceArguments arguments, IDictionary<string, object> routeData) : base(invoker, arguments, routeData)
        {
        }
    }
}