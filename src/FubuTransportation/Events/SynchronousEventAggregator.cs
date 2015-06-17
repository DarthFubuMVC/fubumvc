using System;
using System.Collections.Generic;
using FubuCore.Logging;

namespace FubuTransportation.Events
{
    public class SynchronousEventAggregator : EventAggregator
    {
        public SynchronousEventAggregator(Func<ILogger> logger, IEnumerable<IListener> listeners) : base(logger, listeners)
        {
        }

        public override void SendMessage<T>(T message)
        {
            sendMessageToListeners(message);
        }
    }
}