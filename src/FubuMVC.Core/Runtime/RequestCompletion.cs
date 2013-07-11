using System;
using System.Collections.Generic;

namespace FubuMVC.Core.Runtime
{
    public interface IRequestCompletion
    {
        void IsAsynchronous();
        void Start(Action request);
        void SafeStart(Action request);
        void WhenCompleteDo(Action<Exception> onComplete);
        void Complete();
        void CompleteWithErrors(AggregateException exception);
    }

    public class RequestCompletion : IRequestCompletion
    {
        private readonly Stack<Action<Exception>> _subscribers = new Stack<Action<Exception>>(); 
        private bool _isAsync;

        public void IsAsynchronous()
        {
            _isAsync = true;
        }

        public void Start(Action action)
        {
            action();
            tryComplete();
        }

        public void SafeStart(Action action)
        {
            try
            {
                Start(action);
            }
            catch (Exception ex)
            {
                tryComplete(ex);
            }
        }

        private void tryComplete(Exception ex = null)
        {
            if (_isAsync)
                return;

            complete(ex);
        }

        public void WhenCompleteDo(Action<Exception> onComplete)
        {
            _subscribers.Push(onComplete);
        }

        public void Complete()
        {
            complete();
        }

        public void CompleteWithErrors(AggregateException exception)
        {
            complete(exception);
        }

        private void complete(Exception exception = null)
        {
            while (_subscribers.Count > 0)
            {
                var subscriber = _subscribers.Pop();
                subscriber(exception);
            }
        }
    }
}