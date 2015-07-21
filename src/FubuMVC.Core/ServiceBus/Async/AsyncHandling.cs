using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuCore;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;

namespace FubuMVC.Core.ServiceBus.Async
{
    public class AsyncHandling : IAsyncHandling, IDisposable
    {
        private readonly IInvocationContext _context;
        private readonly IList<Task> _tasks = new List<Task>();
        private readonly IList<Action> _messages = new List<Action>();

        public AsyncHandling(IInvocationContext context)
        {
            _context = context;
        }

        public void Push(Task task)
        {
            _tasks.Add(task);
        }

        public void Push<T>(Task<T> task)
        {
            _messages.Add(() => _context.EnqueueCascading(task.Result));

            _tasks.Add(task);
        }

        public void WaitForAll()
        {
            Task.WaitAll(_tasks.ToArray(), 5.Minutes());
            _messages.Each(x => x());
        }

        // TODO -- need to watch this one.
        public void Dispose()
        {
            _tasks.Each(x => x.SafeDispose());
        }
    }
}