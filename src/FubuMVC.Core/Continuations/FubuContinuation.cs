using System;
using System.Linq.Expressions;
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

        public static FubuContinuation RedirectTo<T>(Expression<Action<T>> expression)
        {
            ActionCall call = ActionCall.For(expression);
            return new FubuContinuation(ContinuationType.Redirect, d => d.RedirectToCall(call))
            {
                _call = call
            };
        }

        public static FubuContinuation RedirectTo(object destination)
        {
            if (destination == null) throw new ArgumentNullException("destination");
            return new FubuContinuation(ContinuationType.Redirect, d => d.RedirectTo(destination))
            {
                _destination = destination
            };
        }

        public static FubuContinuation TransferTo(object destination)
        {
            if (destination == null) throw new ArgumentNullException("destination");
            return new FubuContinuation(ContinuationType.Transfer, d => d.TransferTo(destination))
            {
                _destination = destination
            };
        }

        public static FubuContinuation TransferTo<T>(Expression<Action<T>> expression)
        {
            ActionCall call = ActionCall.For(expression);
            return new FubuContinuation(ContinuationType.Transfer, d => d.TransferToCall(call))
            {
                _call = call
            };
        }

        public static FubuContinuation NextBehavior()
        {
            return new FubuContinuation(ContinuationType.NextBehavior, d => d.InvokeNextBehavior());
        }

        public void AssertWasTransferedTo<T>(Expression<Action<T>> expression)
        {
            ActionCall call = ActionCall.For(expression);
            assertMatches(_type == ContinuationType.Transfer && callMatches(call));
        }

        public void AssertWasTransferedTo<T>(T destination)
        {
            AssertWasTransferedTo<T>(x => x.Equals(destination));
        }

        public void AssertWasTransferedTo<T>(Func<T, bool> predicate)
        {
            assertMatches(_destination != null && typeof (T) == _destination.GetType());
            assertMatches(_type == ContinuationType.Transfer && predicate((T) _destination));
        }

        public void AssertWasRedirectedTo<T>(T destination)
        {
            AssertWasRedirectedTo<T>(x => x.Equals(destination));
        }

        public void AssertWasRedirectedTo<T>(Func<T, bool> predicate)
        {
            assertMatches(_destination != null && typeof(T) == _destination.GetType());
            assertMatches(_type == ContinuationType.Redirect && predicate((T)_destination));
        }

        public void AssertWasRedirectedTo<T>(Expression<Action<T>> expression)
        {
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

            throw new FubuAssertionException(message);
        }

        public void Process(IContinuationDirector director)
        {
            _configure(director);
        }
    }
}