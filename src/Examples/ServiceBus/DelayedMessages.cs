using System;
using System.IO;
using FubuMVC.Core.ServiceBus;

namespace Examples.ServiceBus
{
    public class DoLaterMessage
    {

    }

    // SAMPLE: DelayedMessageSender
    public class DelayedMessageSender
    {
        public void SendDelayedMessage(IServiceBus bus)
        {
            // Send the DoLaterMessage, but delay the processing until
            // the start of the day 5 days from now
            bus.DelaySend(new DoLaterMessage(), DateTime.Today.AddDays(5));



            // Send the DoLaterMessage, but delay the processing for an
            // hour
            bus.DelaySend(new DoLaterMessage(), TimeSpan.FromHours(1));
        }
    }
    // ENDSAMPLE




}