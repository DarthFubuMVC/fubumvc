using System;
using System.Linq.Expressions;
using System.Net;
using FubuCore;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Continuations
{
    // This is what FubuContinuation looks like.  For ease of testing purposes,
    // I put the Assert() methods directly onto FubuContinuation
    public class FubuContinuation
    {
        private readonly Action<IContinuationDirector> _configure;
        private readonly ContinuationType _type;
        private ActionCall _call;
        private object _destination;
        public HttpStatusCode? _statusCode;
        public string _categoryOrHttpMethod;

        private FubuContinuation(ContinuationType type, Action<IContinuationDirector> configure)
        {
            _type = type;
            _configure = configure;
        }

        public ContinuationType Type { get { return _type; } }

        /// <summary>
        /// Only for testing
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T Destination<T>() where T : class
        {
            return _destination as T;
        }

        public static FubuContinuation RedirectTo<T>(Expression<Action<T>> expression, string categoryOrHttpMethod = null)
        {
            ActionCall call = ActionCall.For(expression);
            return new FubuContinuation(ContinuationType.Redirect, d => d.RedirectToCall(call, categoryOrHttpMethod))
            {
                _call = call,
                _categoryOrHttpMethod = categoryOrHttpMethod
            };
        }

        public static FubuContinuation RedirectTo<T>(string categoryOrHttpMethod = null)
            where T : new()
        {
            return RedirectTo(new T(), categoryOrHttpMethod);
        }

        public static FubuContinuation TransferTo<T>(string categoryOrHttpMethod = null)
            where T : new()
        {
            return TransferTo(new T(), categoryOrHttpMethod);
        }

        public static FubuContinuation RedirectTo(object destination, string categoryOrHttpMethod = null)
        {
            if (destination == null) throw new ArgumentNullException("destination");
            return new FubuContinuation(ContinuationType.Redirect, d => d.RedirectTo(destination, categoryOrHttpMethod))
            {
                _destination = destination,
                _categoryOrHttpMethod = categoryOrHttpMethod
            };
        }

        public static FubuContinuation TransferTo(object destination, string categoryOrHttpMethod = null)
        {
            if (destination == null) throw new ArgumentNullException("destination");
            return new FubuContinuation(ContinuationType.Transfer, d => d.TransferTo(destination, categoryOrHttpMethod))
            {
                _destination = destination,
                _categoryOrHttpMethod = categoryOrHttpMethod
            };
        }

        public static FubuContinuation TransferTo<T>(Expression<Action<T>> expression, string categoryOrHttpMethod = null)
        {
            ActionCall call = ActionCall.For(expression);
            return new FubuContinuation(ContinuationType.Transfer, d => d.TransferToCall(call, categoryOrHttpMethod))
            {
                _call = call,
                _categoryOrHttpMethod = categoryOrHttpMethod
            };
        }

        public static FubuContinuation EndWithStatusCode(HttpStatusCode code)
        {
            return new FubuContinuation(ContinuationType.Stop, d => d.EndWithStatusCode(code)){
                _statusCode = code
            };

        }

        public static FubuContinuation NextBehavior()
        {
            return new FubuContinuation(ContinuationType.NextBehavior, d => d.InvokeNextBehavior());
        }

        public void AssertWasTransferedTo<T>(Expression<Action<T>> expression, string categoryOrHttpMethod = null)
        {
            ActionCall call = ActionCall.For(expression);
            assertMatches(_type == ContinuationType.Transfer && callMatches(call) && _categoryOrHttpMethod == categoryOrHttpMethod);
        }

        public void AssertWasTransferedTo<T>(T destination, string categoryOrHttpMethod = null)
        {
            Func<T, bool> predicate = x => x.Equals(destination);
            AssertWasTransferedTo(predicate, categoryOrHttpMethod);
        }

        public void AssertWasTransferedTo<T>(Func<T, bool> predicate, string categoryOrHttpMethod = null)
        {
            assertMatches(_categoryOrHttpMethod == categoryOrHttpMethod);
            assertMatches(_destination != null && typeof (T) == _destination.GetType());
            assertMatches(_type == ContinuationType.Transfer && predicate((T) _destination));
        }

        public void AssertWasRedirectedTo<T>(T destination, string categoryOrHttpMethod = null)
        {
            Func<T, bool> predicate = x => x.Equals(destination);
            AssertWasRedirectedTo(predicate, categoryOrHttpMethod);
        }

        public void AssertWasRedirectedTo<T>(Func<T, bool> predicate, string categoryOrHttpMethod = null)
        {
            assertMatches(_categoryOrHttpMethod == categoryOrHttpMethod);
            assertMatches(_destination != null && typeof(T) == _destination.GetType());
            assertMatches(_type == ContinuationType.Redirect && predicate((T)_destination));
        }

        public void AssertWasRedirectedTo<T>(Expression<Action<T>> expression, string categoryOrHttpMethod = null)
        {
            assertMatches(_categoryOrHttpMethod == categoryOrHttpMethod);
            ActionCall call = ActionCall.For(expression);
            assertMatches(_type == ContinuationType.Redirect && callMatches(call));
        }

        private bool callMatches(ActionCall call)
        {
            return _call != null && call.HandlerType == _call.HandlerType && call.Method == _call.Method;
        }

        public void AssertWasContinuedToNextBehavior()
        {
            assertMatches(_type == ContinuationType.NextBehavior);
        }

        private void assertMatches(bool matches)
        {
            if (matches) return;

            string message = "Assertion Failed!\nContinuation Type:  " + _type;
            if (_destination != null)
            {
                message += "\n  destination model: " + _destination;
            }

            if (_call != null)
            {
                message += "\n destination call: " + _call.Description;
            }

            if (_statusCode.HasValue )
            {
                message += "\n status code:  " + _statusCode.Value.ToString();
            }

            if (_categoryOrHttpMethod.IsNotEmpty())
            {
                message += "\n categoryOrHttpMethod:  " + _categoryOrHttpMethod;
            }

            throw new FubuAssertionException(message);
        }

        public void Process(IContinuationDirector director)
        {
            _configure(director);
        }

        public void AssertWasEndedWithStatusCode(HttpStatusCode httpStatusCode)
        {
            assertMatches(_type == ContinuationType.Stop && _statusCode.HasValue && _statusCode.Value == httpStatusCode);
        }
    }
}