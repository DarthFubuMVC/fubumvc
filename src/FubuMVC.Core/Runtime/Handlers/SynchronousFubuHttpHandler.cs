using System.Collections.Generic;
using System.Web.SessionState;
using FubuCore.Binding;
using StructureMap.Pipeline;

namespace FubuMVC.Core.Runtime.Handlers
{
    public class SynchronousFubuHttpHandler : SessionLessFubuHttpHandler, IRequiresSessionState
    {
        public SynchronousFubuHttpHandler(IBehaviorInvoker invoker, TypeArguments arguments, IDictionary<string, object> routeData) : base(invoker, arguments, routeData)
        {
        }
    }
}