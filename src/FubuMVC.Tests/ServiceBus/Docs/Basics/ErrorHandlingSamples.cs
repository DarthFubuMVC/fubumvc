using System;
using System.IO;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus.Configuration;

namespace FubuMVC.Tests.ServiceBus.Docs.Basics
{
    // SAMPLE: ErrorHandlingPolicySample
    public class ErrorHandlingPolicy : HandlerChainPolicy
    {
        public override void Configure(HandlerChain handlerChain)
        {
            //when retrying, do so a maximum of 5 times
            handlerChain.MaximumAttempts = 5;

            //retry now
            handlerChain.OnException<ConcurrencyException>()
                .Retry();

            //retry again 5 seconds from now
            handlerChain.OnException<ConcurrencyException>()
                .RetryLater(TimeSpan.FromSeconds(5));

            //immediately move the error and original message to
            //the error queue
            handlerChain.OnException<FileNotFoundException>()
                .MoveToErrorQueue();

            //retries, but puts at the end of the line allowing other
            //messages to be processed
            handlerChain.OnException<Exception>()
                .Requeue();
        }
    }
    // ENDSAMPLE

    // SAMPLE: ErrorHandlingTransportSample
    public class TransportRegistryWithErrorPolicy : FubuRegistry
    {
        public TransportRegistryWithErrorPolicy()
        {
            //applies to all handler chains
            Policies.Global.Add<ErrorHandlingPolicy>();

            //applies policy only to chains created from this registry
            Policies.Local.Add<ErrorHandlingPolicy>();
        }
    }
    // ENDSAMPLE

    public class ConcurrencyException : Exception
    {
    }
}