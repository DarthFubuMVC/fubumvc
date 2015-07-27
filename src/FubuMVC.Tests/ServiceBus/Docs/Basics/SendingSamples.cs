using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Runtime.Cascading;
using FubuMVC.Core.ServiceBus.Web;

namespace FubuMVC.Tests.ServiceBus.Docs.Basics
{
    // SAMPLE: SendingSamples
    public class SendingSamples
    {
        private readonly IServiceBus _bus;

        public SendingSamples(IServiceBus bus)
        {
            _bus = bus;
        }

        public void FireAndForget()
        {
            _bus.Send(new MessageToSend());
        }

        public async Task SendWithAcknowledgement()
        {
            //Don't care about a specific response message, but rather acknowledgement
            //that message was processed
            await _bus.SendAndWait(new MessageToSend());
        }

        public void SendDelayedMessage()
        {
            _bus.DelaySend(new MessageToSend(), TimeSpan.FromMinutes(15));
        }

        public async Task<string> SendAndReply()
        {
            var response = await _bus.Request<Response>(new ExpectResponse());
            return response.Message;
        }
    }
    // ENDSAMPLE

    public class ExpectResponse
    {
    }

    public class Response
    {
        public string Message { get; set; }
    }

    public class NotResponding
    {
    }

    public class MessageToSend
    {
    }

    // SAMPLE: SendingFromHandlerSamples
    public class SendingFromHandler
    {
        public RespondToSender HandleWithReply(ExpectResponse message)
        {
            //Send message only to the original sender
            return new RespondToSender(new Response());
        }

        public Response HandleWithReplyOrPublish(MessageToSend message)
        {
            //Will reply to original sender if sender used bus.Request<Response>(message)
            //Will also send message through routing rules in your TransportRegistry
            return new Response();
        }

        public DelayedResponse HandleWithDelayedResponse(MessageToSend message)
        {
            //Route the message, but don't process it for 15 minutes
            return new DelayedResponse(new Response(), TimeSpan.FromMinutes(15));
        }

        public IEnumerable<object> HandleAndSpawnMany(MessageToSend message)
        {
            yield return new RespondToSender(new Response());
            yield return new NotResponding();
            yield return new DelayedResponse(new NotResponding(), TimeSpan.FromMinutes(1));
        }
    }
    // ENDSAMPLE

    public class LoginInputModel
    {
    }

    // SAMPLE: EndpointSendingSample
    public class LoginEndpoint : ISendMessages
    {
        public MessageToSend post_my_route(LoginInputModel input)
        {
            //Send message using routing rules in TransportRegistry
            return new MessageToSend();
        }
    }
    // ENDSAMPLE
}