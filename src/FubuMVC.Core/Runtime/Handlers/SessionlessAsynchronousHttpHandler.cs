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

        public void ProcessRequest(System.Web.HttpContext context)
        {
            throw new InvalidOperationException("Synchronous requests are not supported with this handler");
        }

        public bool IsReusable
        {
            get { return false; }
        }

        public IAsyncResult BeginProcessRequest(System.Web.HttpContext context, AsyncCallback cb, object extraData)
        {
            var taskCompletionSource = new TaskCompletionSource<object>();
            var requestCompletion = new RequestCompletion();
            requestCompletion.WhenCompleteDo(ex =>
            {
                if (ex != null)
                {
                    taskCompletionSource.SetException(ex);
                }
                taskCompletionSource.SetResult(null);
                cb(taskCompletionSource.Task);
            });
            requestCompletion.Start(() => _invoker.Invoke(_arguments, _routeData, requestCompletion));

            return taskCompletionSource.Task;
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            var task = (Task) result;
            task.Wait(); //This won't really block because the request is already complete
        }
    }
}