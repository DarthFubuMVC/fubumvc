using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FubuMVC.Core.ServiceBus;
using NUnit.Framework;

namespace FubuMVC.Tests.ServiceBus.Monitoring
{
    public class RiggedServiceBus : IServiceBus
    {
        private readonly IList<RequestReplyExpectation> _expectations = new List<RequestReplyExpectation>();


        public class RequestReplyExpectation : FromExpression, ReturnsExpression
        {
            public Uri Destination;
            public object Message;
            public object Response;

            public ReturnsExpression AtDestination(Uri destination)
            {
                Destination = destination;
                return this;
            }

            public void Returns(object response)
            {
                Response = response;
            }

            public void Throws(Exception ex)
            {
                Exception = ex;
            }

            public Exception Exception { get; set; }

            public bool WasCalled { get; set; }

            public override string ToString()
            {
                return string.Format("Expected message {0} to Destination: {1}", Message, Destination);
            }
        }

        public FromExpression ExpectMessage(object message)
        {
            var expectation = new RequestReplyExpectation
            {
                Message = message
            };

            _expectations.Add(expectation);

            return expectation;
        }

        public interface FromExpression
        {
            ReturnsExpression AtDestination(Uri destination);
        }

        public interface ReturnsExpression
        {
            void Returns(object response);
            void Throws(Exception ex);
        }

        public Task<TResponse> Request<TResponse>(object request, RequestOptions options = null)
        {
            var expectation =
                _expectations.FirstOrDefault(x => x.Message.Equals(request) && x.Destination == options.Destination);

            if (expectation == null)
                Assert.Fail("No expectation for message {0} to destination {1}", request, options.Destination);

            var completion = new TaskCompletionSource<TResponse>();

            if (expectation.Exception != null)
            {
                completion.SetException(expectation.Exception);
            }
            else
            {
                completion.SetResult((TResponse)expectation.Response);
            }

            expectation.WasCalled = true;
            

            return completion.Task;
        }

        public void AssertThatAllExpectedMessagesWereReceived()
        {
            var missing = _expectations.Where(x => !x.WasCalled);

            if (missing.Any())
            {
                var message = missing.Select(x => x.ToString()).Join("\n");
                Assert.Fail(message);
            }
        }

        public void Send<T>(T message)
        {
            throw new NotImplementedException();
        }

        public void Send<T>(Uri destination, T message)
        {
            throw new NotImplementedException();
        }

        public void Consume<T>(T message)
        {
            throw new NotImplementedException();
        }

        public void DelaySend<T>(T message, DateTime time)
        {
            throw new NotImplementedException();
        }

        public void DelaySend<T>(T message, TimeSpan delay)
        {
            throw new NotImplementedException();
        }

        public Task SendAndWait<T>(T message)
        {
            throw new NotImplementedException();
        }

        public Task SendAndWait<T>(Uri destination, T message)
        {
            throw new NotImplementedException();
        }

        public Task RemoveSubscriptionsForThisNodeAsync()
        {
            throw new NotImplementedException();
        }
    }
}
