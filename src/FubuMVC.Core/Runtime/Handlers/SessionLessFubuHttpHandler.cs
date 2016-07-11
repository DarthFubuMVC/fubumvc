using System.Collections.Generic;
using System.Web;
using StructureMap.Pipeline;

namespace FubuMVC.Core.Runtime.Handlers
{
    public class SessionLessFubuHttpHandler : IHttpHandler
    {
        private readonly IBehaviorInvoker _invoker;
        private readonly TypeArguments _arguments;
        private readonly IDictionary<string, object> _routeData;

        public SessionLessFubuHttpHandler(IBehaviorInvoker invoker, TypeArguments arguments, IDictionary<string, object> routeData)
        {
            _invoker = invoker;
            _arguments = arguments;
            _routeData = routeData;
        }

        public void ProcessRequest(System.Web.HttpContext context)
        {
            var requestCompletion = new RequestCompletion();
            requestCompletion.Start(() => _invoker.Invoke(_arguments, _routeData, requestCompletion));
        }

        public bool IsReusable => false;
    }
}