using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Services.Messaging;
using FubuMVC.Tests.TestSupport;
using Shouldly;
using Xunit;

namespace FubuMVC.Tests.Services.Messaging
{
    
    public class EventAggregatorTester : IDisposable
    {
        private RecordingListener theListener;

        public EventAggregatorTester()
        {
            theListener = new RecordingListener();
            var hub = new MessagingHub();
            hub.AddListener(theListener);

            var remoteListener = new RemoteListener(hub);
            GlobalMessageTracking.Start(remoteListener);
        }

        public void Dispose()
        {
            GlobalMessageTracking.Stop();
        }

        [Fact]
        public void send_message_by_category()
        {
            GlobalMessageTracking.SendMessage("category1", "some message");

            var expected = new ServiceMessage
            {
                Category = "category1", Message = "some message"
            };

            Wait.Until(() => theListener.Received.Contains(expected));

            theListener.Received.OfType<ServiceMessage>().Single()
                       .ShouldBe(expected);

        }
    }

    public class RecordingListener : IListener
    {
        private readonly IList<object> _received = new List<object>();

        public IEnumerable<object> Received
        {
            get { return _received; }
        }

        public void Clear()
        {
            _received.Clear();
        }

        public void Receive<T>(T message)
        {
            _received.Add(message);
        }

        
    }
}