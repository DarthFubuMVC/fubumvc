using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Http;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Registration.Nodes;
using FubuMVC.Core.ServiceBus.ErrorHandling;
using FubuMVC.Core.ServiceBus.Registration.Nodes;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;

namespace FubuMVC.Core.ServiceBus.Configuration
{
    public class HandlerChain : BehaviorChain, IMayHaveInputType
    {
        public readonly IList<IErrorHandler> ErrorHandlers = new List<IErrorHandler>();
        public int MaximumAttempts = 1;

        public HandlerChain()
        {
        }

        public HandlerChain(IEnumerable<HandlerCall> calls) : this()
        {
            calls.Each(AddToEnd);
        }

        public void ReadAttributes()
        {
            this.OfType<HandlerCall>().ToArray().Each(call =>
            {
                call.ForAttributes<ModifyChainAttribute>(att => att.Alter(call));
            });
        }

        public virtual bool IsPollingJob()
        {
            return false;
        }

        internal protected override void InsertNodes(ConnegSettings settings)
        {
            // do nothing.
        }

        public ContinuationExpression OnException<T>() where T : Exception
        {
            return new OnExceptionExpression<T>(this);
        }

        public interface ThenContinueExpression
        {
            ContinuationExpression Then { get; }
        }

        public interface ContinuationExpression
        {
            ThenContinueExpression Retry();
            ThenContinueExpression Requeue();
            ThenContinueExpression MoveToErrorQueue();
            ThenContinueExpression RetryLater(TimeSpan delay);
            ThenContinueExpression ContinueWith(IContinuation continuation);
            ThenContinueExpression ContinueWith<TContinuation>() where TContinuation : IContinuation, new();
            ThenContinueExpression RespondWithMessage(Func<Exception, Envelope, object> messageFunc);
        }

        public class OnExceptionExpression<T> : ContinuationExpression, ThenContinueExpression where T : Exception
        {
            private readonly HandlerChain _parent;
            private readonly Lazy<ErrorHandler> _handler; 

            public OnExceptionExpression(HandlerChain parent)
            {
                _parent = parent;

                _handler = new Lazy<ErrorHandler>(() => {
                    var handler = new ErrorHandler();
                    handler.AddCondition(new ExceptionTypeMatch<T>());
                    _parent.ErrorHandlers.Add(handler);

                    return handler;
                });
            }

            public ThenContinueExpression Retry()
            {
                return ContinueWith(new RetryNowContinuation());
            }

            public ThenContinueExpression Requeue()
            {
                return ContinueWith(new RequeueContinuation());
            }

            public ThenContinueExpression MoveToErrorQueue()
            {
                _parent.ErrorHandlers.Add(new MoveToErrorQueueHandler<T>());

                return this;
            }

            public ThenContinueExpression RetryLater(TimeSpan delay)
            {
                return ContinueWith(new DelayedRetryContinuation(delay));
            }

            public ThenContinueExpression ContinueWith(IContinuation continuation)
            {
                _handler.Value.AddContinuation(continuation);

                return this;
            }

            public ThenContinueExpression ContinueWith<TContinuation>() where TContinuation : IContinuation, new()
            {
                return ContinueWith(new TContinuation());
            }

            ContinuationExpression ThenContinueExpression.Then
            {
                get
                {
                    return this;
                }
            }

            public ThenContinueExpression RespondWithMessage(Func<Exception, Envelope, object> messageFunc)
            {
                var handler = new RespondWithMessageHandler<T>(messageFunc);
                _parent.ErrorHandlers.Add(handler);

                return this;
            }
        }

        public bool IsAsync
        {
            get
            {
                return this.OfType<HandlerCall>().Any(x => x.IsAsync);
            }
        }
    }
}