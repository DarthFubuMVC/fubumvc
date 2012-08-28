using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using FubuCore.Binding;

namespace FubuMVC.Core.Runtime.Handlers
{
    public class SessionlessAsynchronousHttpHandler : IHttpAsyncHandler
    {
        private readonly IBehaviorInvoker _invoker;
        private readonly ServiceArguments _arguments;
        private readonly IDictionary<string, object> _routeData;

        public SessionlessAsynchronousHttpHandler(IBehaviorInvoker invoker, ServiceArguments arguments, IDictionary<string, object> routeData)
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
            var exceptionHandlingObserver = new ExceptionHandlingObserver();
            _arguments.Set(typeof(IExceptionHandlingObserver), exceptionHandlingObserver);
            var task = Task.Factory.StartNew(state => _invoker.Invoke(_arguments, _routeData), exceptionHandlingObserver);
            task.ContinueWith(x => cb(x));
            return task;
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            var task = (Task) result;
            var exceptionHandlingObserver = (IExceptionHandlingObserver) result.AsyncState;
            task.FinishProcessingTask(exceptionHandlingObserver);
        }
    }
}