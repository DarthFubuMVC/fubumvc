using System;
using FubuCore;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using FubuMVC.Core.ServiceBus.Runtime.Serializers;
using FubuMVC.Core.ServiceBus.Subscriptions;
using Rhino.Mocks;
using TestMessages.ScenarioSupport;

namespace FubuMVC.Tests.ServiceBus
{
    public static class ObjectMother
    {
         public static Envelope Envelope()
         {
             return new Envelope
             {
                 Data = new byte[] { 1, 2, 3, 4 },
                 Callback = MockRepository.GenerateMock<IMessageCallback>()
             };
         }

        public static Envelope EnvelopeWithMessage()
        {
            var envelope = Envelope();
            envelope.Message = new Message1();

            return envelope;
        }

        public static Envelope EnvelopeWithSerializationError()
        {
            var envelope = Envelope();
            envelope.UseSerializer(new ThrowingEnvelopeSerializer());
            return envelope;
        }

        public static InvocationContext InvocationContext()
        {
            var envelope = Envelope();
            envelope.Message = new Message();

            return new InvocationContext(envelope, new HandlerChain());
        }

        public static Subscription NewSubscription(string nodeName = null)
        {
            return new Subscription
            {
                MessageType = Guid.NewGuid().ToString(),
                NodeName = nodeName ?? "TheNode",
                Receiver = "memory://receiver".ToUri(),
                Source = "memory://source".ToUri(),
                Role = SubscriptionRole.Subscribes

            };
        }

        public static Subscription ExistingSubscription(string nodeName = null)
        {
            var subscription = NewSubscription();
            subscription.Id = Guid.NewGuid();

            if (nodeName.IsNotEmpty())
            {
                subscription.NodeName = nodeName;
            }

            return subscription;
        }


    }

    public class ThrowingEnvelopeSerializer : IEnvelopeSerializer
    {
        public object Deserialize(Envelope envelope)
        {
            throw new EnvelopeDeserializationException("Error");
        }

        public void Serialize(Envelope envelope, ChannelNode node)
        {
            throw new EnvelopeDeserializationException("Error");
        }
    }
}