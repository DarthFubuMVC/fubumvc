using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Diagnostics;
using FubuMVC.Core.ServiceBus.Events;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using StructureMap;

namespace Serenity.ServiceBus
{
    public class ExternalNode : IDisposable
    {
        private readonly string _name;
        private readonly ChannelGraph _systemUnderTest;
        private readonly Type _registryType;
        private bool _isStarted;
        private IMessagingSession _messageListener;
        private IMessageRecorder _recorder;

        public ExternalNode(string name, Type registryType, ChannelGraph systemUnderTest)
        {
            _registryType = registryType;
            _name = name;
            _systemUnderTest = systemUnderTest;

            if (!IsValidRegistryType(_registryType))
            {
                throw new ArgumentException("Registry type must extend FubuTransportRegistry", "registryType");
            }
        }

        private bool IsValidRegistryType(Type type)
        {
            return type.IsConcreteTypeOf<FubuRegistry>();
        }

        public FubuRuntime Runtime { get; private set; }
        public Uri Uri { get; private set; }

        public void ClearReceivedMessages()
        {
            _recorder.Clear();
        }

        public void Dispose()
        {
            _isStarted = false;
            if (Runtime != null)
            {
                FubuMVC.Core.Services.Messaging.EventAggregator.Messaging.RemoveListener(_messageListener);
                Runtime.Dispose();
                Runtime = null;
            }
        }

        public bool ReceivedMessage<T>(Func<T, bool> predicate = null)
        {
            return _recorder.ReceivedMessages
                .Any(x => x.GetType().CanBeCastTo<T>()
                          && (predicate == null || predicate(x.As<T>())));
        }

        public IEnumerable<T> ReceivedMessages<T>()
        {
            return _recorder.ReceivedMessages
                .OfType<T>();
        }

        /// <summary>
        /// Sends a message from this node to the system under test.
        /// </summary>
        public void Send<T>(T message)
        {
            var channelNode = _systemUnderTest.FirstOrDefault(x => x.Publishes(typeof(T)));
            if (channelNode == null)
                throw new ArgumentException("Cannot find destination channel for message type {0}. Have you configured the channel with AcceptsMessage()?".ToFormat(typeof(T)), "message");

            Uri destination = channelNode.Uri;
            var bus = Runtime.Get<IServiceBus>();
            bus.Send(destination, message);
        }

        /// <summary>
        /// Simulate a response from this endpoint to the last received request of type TRequest.
        /// An example can be found in FubuTransportation.Serenity.Samples.
        /// </summary>
        /// <param name="message">The response message.</param>
        public void RespondToRequestWithMessage<TRequest>(object message)
        {
            var request = _recorder.ReceivedEnvelopes
                .LastOrDefault(x => x.Message is TRequest);
            if (request == null)
            {
                throw new InvalidOperationException("This node hasn't received a message of type {0}".ToFormat(typeof(TRequest)));
            }

            SendResponseMessage(message, request);
        }

        /// <summary>
        /// Simulate a response to a received request from this endpoint.
        /// </summary>
        /// <param name="requestSelector">Given the envelopes received by this endpoint, selects the envelope the response is for.</param>
        /// <param name="message">The response message.</param>
        public void RespondToRequestWithMessage(
            Func<IEnumerable<EnvelopeToken>, EnvelopeToken> requestSelector,
            object message)
        {
            var request = requestSelector(_recorder.ReceivedEnvelopes);
            SendResponseMessage(message, request);
        }

        private void SendResponseMessage(object message, EnvelopeToken request)
        {
            var sender = Runtime.Get<IEnvelopeSender>();
            var response = new Envelope
            {
                Message = message,
                Destination = request.ReplyUri,
                OriginalId = request.OriginalId ?? request.CorrelationId,
                ParentId = request.CorrelationId,
                ResponseId = request.CorrelationId
            };
            sender.Send(response);
        }

        public void Start()
        {
            if (_isStarted)
                return;
            _isStarted = true;

            var registry = Activator.CreateInstance(_registryType).As<FubuRegistry>();
            registry.NodeName = _name;
            registry.ServiceBus.EnableInMemoryTransport();
            registry.Services.ReplaceService<IEnvelopeHandler, ExternalNodeEnvelopeHandler>();
            TestNodes.Alterations.Each(x => x(registry));


            var container = new Container(x =>
            {
                x.ForSingletonOf<IMessageRecorder>().Use<MessageRecorder>();
                x.Forward<IMessageRecorder, IListener>();
            });

            registry.StructureMap(container);

            Runtime = registry.ToRuntime();
            Uri = Runtime.Get<ChannelGraph>().ReplyUriList().First();
            _recorder = Runtime.Get<IMessageRecorder>();

            // Wireup the messaging session so the MessageHistory gets notified of messages on this node
            _messageListener = Runtime.Get<IMessagingSession>();
            FubuMVC.Core.Services.Messaging.EventAggregator.Messaging.AddListener(_messageListener);
        }
    }

    // Prevents our fake node from returning FailureAcknowledgements because we don't 
    // have actual message handlers on this end.
    public class ExternalNodeEnvelopeHandler : SimpleEnvelopeHandler
    {
        public override bool Matches(Envelope envelope)
        {
            return true;
        }

        public override void Execute(Envelope envelope, IEnvelopeContext context)
        {
            envelope.Callback.MarkSuccessful();
        }
    }
}