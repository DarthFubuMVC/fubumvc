using System;
using System.Collections.Generic;

namespace FubuMVC.Core.Runtime
{
    public class ExceptionHandlingObserver : IExceptionHandlingObserver
    {
        private readonly IList<Exception> _handled = new List<Exception>();

        public void RecordHandled(Exception exception)
        {
            _handled.Add(exception);
        }

        public bool WasObserved(Exception exception)
        {
            return _handled.Contains(exception);
        }
    }

    public interface IExceptionHandlingObserver
    {
        void RecordHandled(Exception exception);
        bool WasObserved(Exception exception);
    }
}