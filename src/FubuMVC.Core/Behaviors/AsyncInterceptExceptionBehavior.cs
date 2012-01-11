﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Behaviors
{
    public class AsyncInterceptExceptionBehavior : IActionBehavior
    {
        private readonly IEnumerable<IExceptionHandler> _exceptionHandlers;
        private readonly IExceptionHandlingObserver _exceptionHandlingObserver;

        public AsyncInterceptExceptionBehavior(IEnumerable<IExceptionHandler> exceptionHandlers, IExceptionHandlingObserver exceptionHandlingObserver)
        {
            _exceptionHandlers = exceptionHandlers;
            _exceptionHandlingObserver = exceptionHandlingObserver;
        }

        public IActionBehavior InsideBehavior { get; set; }

        public void Invoke()
        {
            if (InsideBehavior == null)
                throw new FubuAssertionException("When intercepting exceptions you must have an inside behavior. Otherwise, there would be nothing to intercept.");

            var task = Task.Factory.StartNew(() => InsideBehavior.Invoke(), TaskCreationOptions.AttachedToParent);
            task.ContinueWith(x =>
            {
                try
                {
                    //this will allow a catching an exception rather than inspecting task data
                    x.Wait();
                }
                catch (AggregateException e)
                {
                    var aggregateException = e.Flatten();
                    aggregateException.InnerExceptions.Each(TryHandle);
                }
            }, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.AttachedToParent);
        }

        public void InvokePartial()
        {
            InsideBehavior.InvokePartial();
        }

        private void TryHandle(Exception exception)
        {
            var handlers =_exceptionHandlers.Where(x => x.ShouldHandle(exception)).ToList();
            if(handlers.Count == 0)
            {
                return;
            }

            handlers.Each(x => x.Handle(exception));
            _exceptionHandlingObserver.RecordHandled(exception);
        }
    }
}