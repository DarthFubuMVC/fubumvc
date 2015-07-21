using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Headers;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;

namespace FubuTransportation.Testing
{
    public class RecordingReceiver : IReceiver
    {
        // An immutable list would be useful here, if we were using the immutable library.
        private readonly IList<Envelope> _received = new List<Envelope>(); 

        public IList<Envelope> Received
        {
            get
            {
                lock (_received)
                {
                    return _received.ToList(); 
                }
            }
        }

        public void Receive(byte[] data, IHeaders headers, IMessageCallback callback)
        {
            var envelope = new Envelope(data, headers, callback);
            lock (_received)
            {
                _received.Add(envelope); 
            }

            envelope.Callback.MarkSuccessful();
        }
    }
}