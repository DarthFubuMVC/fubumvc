using System;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using FubuCore;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Headers;
using LightningQueues;

namespace FubuMVC.LightningQueues
{
    public class LightningQueuesChannel : IChannel
    {
        public static string MaxAttemptsHeader = "max-delivery-attempts";
        public static string DeliverByHeader = "deliver-by";
        private readonly string _queueName;
        private readonly Queue _queueManager;
        private IDisposable _disposable;

        public static LightningQueuesChannel BuildPersistentChannel(LightningUri uri, IPersistentQueues queues, int mapSize, int maxDatabases, bool incoming)
        {
            var queueManager = queues.PersistentManagerFor(uri.Port, incoming, mapSize, maxDatabases);
            return new LightningQueuesChannel(uri.Address, uri.QueueName, queueManager);
        }

        public static LightningQueuesChannel BuildNoPersistenceChannel(LightningUri uri, IPersistentQueues queues, bool incoming)
        {
            var queueManager = queues.NonPersistentManagerFor(uri.Port, incoming);
            return new LightningQueuesChannel(uri.Address, uri.QueueName, queueManager);
        }

        public LightningQueuesChannel(Uri address, string queueName, Queue queueManager)
        {
            Address = address;
            _queueName = queueName;
            _queueManager = queueManager;
            _disposable = Disposable.Empty;
        }

        public Uri Address { get; }

        public ReceivingState Receive(IReceiver receiver)
        {
            _disposable = _queueManager.Receive(_queueName).Subscribe(message =>
            {
                Task.Run(() =>
                {
                    receiver.Receive(message.Message.Data, new DictionaryHeaders(message.Message.Headers),
                        new TransactionCallback(message.QueueContext, message.Message));
                });
            });

            return ReceivingState.StopReceiving;
        }

        public void Send(byte[] data, IHeaders headers)
        {
            _queueManager.Send(data, headers, Address, _queueName);
        }


        public void Dispose()
        {
            _disposable.Dispose();
        }
    }

    public static class MessageExtensions
    {
        public static Envelope ToEnvelope(this Message message)
        {
            var envelope = new Envelope(new DictionaryHeaders(message.Headers))
            {
                Data = message.Data
            };

            return envelope;
        }

        public static EnvelopeToken ToToken(this Message message)
        {
            return new EnvelopeToken
            {
                Data = message.Data,
                Headers = new DictionaryHeaders(message.Headers)
            };
        }

        public static Message Copy(this Message message)
        {
            var copy = new Message
            {
                Data = message.Data,
                Headers = message.Headers,
            };

            return copy;
        }

        public static DateTime ExecutionTime(this Message message)
        {
            return message.ToEnvelope().ExecutionTime.Value;
        }

        public static void TranslateHeaders(this OutgoingMessage messagePayload)
        {
            string headerValue;
            messagePayload.Headers.TryGetValue(LightningQueuesChannel.MaxAttemptsHeader, out headerValue);
            if (headerValue.IsNotEmpty())
            {
                messagePayload.MaxAttempts = int.Parse(headerValue);
            }
            messagePayload.Headers.TryGetValue(LightningQueuesChannel.DeliverByHeader, out headerValue);
            if (headerValue.IsNotEmpty())
            {
                messagePayload.DeliverBy = DateTime.Parse(headerValue);
            }
        }

        public static void Send(this Queue queueManager, byte[] data, IHeaders headers, Uri address, string queueName)
        {
            var messagePayload = new OutgoingMessage
            {
                Id = MessageId.GenerateRandom(),
                Data = data,
                Headers = headers.ToDictionary(),
                SentAt = DateTime.Now,
                Destination = address,
                Queue = queueName,
            };
            //TODO Maybe expose something to modify transport specific payloads?
            messagePayload.TranslateHeaders();


            queueManager.Send(messagePayload);
        }
    }
}