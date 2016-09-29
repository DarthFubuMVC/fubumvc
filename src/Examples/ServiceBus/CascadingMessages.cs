using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FubuMVC.Core.Diagnostics;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Cascading;

namespace Examples.ServiceBus
{
    public class MyMessage
    {

    }

    public class MyResponse
    {

    }


    // SAMPLE: NoCascadingHandler
    public class NoCascadingHandler
    {
        private readonly IServiceBus _bus;

        public NoCascadingHandler(IServiceBus bus)
        {
            _bus = bus;
        }

        public void Consume(MyMessage message)
        {
            // do whatever work you need to for MyMessage,
            // then send out a new MyResponse
            _bus.Send(new MyResponse());
        }
    }
    // ENDSAMPLE

    // SAMPLE: CascadingHandler
    public class CascadingHandler
    {
        public MyResponse Consume(MyMessage message)
        {
            return new MyResponse();
        }
    }
    // ENDSAMPLE

    // SAMPLE: Request/Replay-with-cascading
    public class Requester
    {
        private readonly IServiceBus _bus;

        public Requester(IServiceBus bus)
        {
            _bus = bus;
        }

        public Task<MyResponse> GatherResponse()
        {
            return _bus.Request<MyResponse>(new MyMessage());
        }
    }
    // ENDSAMPLE

    public class GoNorth {}

    public class GoSouth {}

    public class GoWest {}

    public class GoEast {}

    public class DirectionRequest
    {
        public string Direction;
    }

    // SAMPLE: ConditionalResponseHandler
    public class ConditionalResponseHandler
    {
        public object Consume(DirectionRequest request)
        {
            switch (request.Direction)
            {
                case "North":
                    return new GoNorth();
                case "South":
                    return new GoSouth();
            }

            // This does nothing
            return null;
        }
    }
    // ENDSAMPLE

    // SAMPLE: DelayedResponseHandler
    public class DelayedResponseHandler
    {
        public DelayedResponse Consume(DirectionRequest request)
        {
            // Process GoWest in 5 minutes from now
            return new DelayedResponse(new GoWest(), TimeSpan.FromMinutes(5));
        }

        public DelayedResponse Consume(MyMessage message)
        {
            // Process GoEast at 8 PM local time
            return new DelayedResponse(new GoEast(), DateTime.Today.AddHours(20));
        }
    }
    // ENDSAMPLE

    // SAMPLE: MultipleResponseHandler
    public class MultipleResponseHandler
    {
        public IEnumerable<object> Consume(MyMessage message)
        {
            // Go North now
            yield return new GoNorth();

            // Go West in an hour
            yield return new DelayedResponse(new GoWest(), TimeSpan.FromHours(1));
        }
    }
    // ENDSAMPLE


    // SAMPLE: BackToSenderHandler
    public class BackToSenderHandler
    {
        public RespondToSender Consume(MyMessage message)
        {
            return new RespondToSender(new GoWest());
        }
    }
    // ENDSAMPLE

    // SAMPLE: GoDirectlyHandler
    public class GoDirectlyHandler
    {
        public static readonly Uri ChannelAddress
            = "lq.tcp://localhost:2200/service".ToUri();

        public SendDirectlyTo Consume(MyMessage message)
        {
            // Send the GoWest message to the running node
            // at a given Uri
            return new SendDirectlyTo(ChannelAddress, new GoWest());
        }
    }
    // ENDSAMPLE

    // SAMPLE: RespondsHandler
    public class RespondsHandler
    {
        public Respond Consume(MyMessage message)
        {
            return Respond
                // The actual message being send back out
                .With(new GoEast())

                // Delay the processing by a timespan
                .DelayedBy(TimeSpan.FromDays(1))

                // Delay the processing until a given time
                .DelayedUntil(DateTime.Today.AddDays(3))

                // Send this message directly to the originator
                // of the original MyMessage
                .ToSender()

                // Finally, directly modify the Envelope for
                // the outgoing message for rarely used options
                .Altered(envelope =>
                {
                    // Do any alterations you'd want to the outgoing message

                    // Override the serialization maybe?
                    envelope.ContentType = "application/json";

                    envelope.Destination
                        = new Uri("lq.tcp://localhost:2201/system");
                });
        }
    }
    // ENDSAMPLE

    // SAMPLE: ISendMyself-Specially
    public class SpecialGoWest : ISendMyself
    {
        public Envelope CreateEnvelope(Envelope original)
        {
            return new Envelope
            {
                Message = new GoWest(),
                Destination = original.ReplyUri,
                ContentType = "text/xml"
            };
        }
    }
    // ENDSAMPLE


}