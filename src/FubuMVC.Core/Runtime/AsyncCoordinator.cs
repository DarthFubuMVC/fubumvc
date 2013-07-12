using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FubuMVC.Core.Runtime
{
    public interface IAsyncCoordinator
    {
        void Push(params Task[] task);
    }

    public class AsyncCoordinator : IAsyncCoordinator
    {
        private readonly IList<Exception> _unhandledExceptions = new List<Exception>(); 
        private readonly IRequestCompletion _requestCompletion;
        private readonly IEnumerable<IExceptionHandler> _exceptionHandlers;
        private int _pendingOperations;
        private readonly object _sync = new object();

        public AsyncCoordinator(IRequestCompletion requestCompletion, IEnumerable<IExceptionHandler> exceptionHandlers)
        {
            _requestCompletion = requestCompletion;
            _exceptionHandlers = exceptionHandlers;
            _requestCompletion.IsAsynchronous();
        }

        public void Push(params Task[] tasks)
        {
            inLock(() => _pendingOperations += tasks.Length);
            Task.Factory.ContinueWhenAll(tasks, _ =>
            {
                tasks.Each(x => inLock(() => taskCompleted(x)));
            }, TaskContinuationOptions.ExecuteSynchronously);
        }

        private void taskCompleted(Task task)
        {
            _pendingOperations--;
            checkForErrors(task);
            checkForCompletion();
        }

        private void checkForCompletion()
        {
            if (_pendingOperations == 0)
            {
                if (_unhandledExceptions.Count == 0)
                {
                    _requestCompletion.Complete();
                }
                else
                {
                    _requestCompletion.CompleteWithErrors(new AggregateException(_unhandledExceptions));
                }
            }
        }

        private void checkForErrors(Task task)
        {
            if (task.IsFaulted)
            {
                var innerExceptions = task.Exception.Flatten().InnerExceptions;
                innerExceptions.Each(ex =>
                {
                    bool observed = false;
                    _exceptionHandlers.Where(x => x.ShouldHandle(ex)).Each(x =>
                    {
                        observed = true;
                        x.Handle(ex);
                    });
                    if (!observed)
                    {
                        _unhandledExceptions.Add(ex);
                    }
                });
            }
        }

        private void inLock(Action action)
        {
            lock (_sync)
            {
                action();
            }
        }
    }
}