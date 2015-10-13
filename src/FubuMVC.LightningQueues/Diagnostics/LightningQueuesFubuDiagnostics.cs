using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using FubuCore;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.ServiceBus.ErrorHandling;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Headers;
using FubuMVC.Core.ServiceBus.Runtime.Serializers;
using FubuMVC.Core.Services;
using LightningQueues.Model;

namespace FubuMVC.LightningQueues.Diagnostics
{
    public class LightningQueuesFubuDiagnostics
    {
        private readonly IPersistentQueues _queues;
        private readonly IQueueMessageRetrieval _queueMessageRetrieval;
        private readonly IEnvelopeSerializer _serializer;

        public LightningQueuesFubuDiagnostics(IPersistentQueues queues, IQueueMessageRetrieval queueMessageRetrieval, IEnvelopeSerializer serializer)
        {
            _queues = queues;
            _queueMessageRetrieval = queueMessageRetrieval;
            _serializer = serializer;
        }

        public QueueManagersVisualization get_queues()
        {
            var visualization = new QueueManagersVisualization
            {
                QueueManagers = _queues.AllQueueManagers.Select(x => new QueueManagerModel(x)).ToList()
            };

            return visualization;
        }


        public QueueMessagesVisualization get_messages_Port_QueueName(MessagesInputModel input)
        {
            var request = new QueueMessageRetrievalRequest
            {
                Port = input.Port,
                QueueName = input.QueueName
            };

            var messages = _queueMessageRetrieval.GetAllMessagesInQueue(request).Select(msg =>
            {
                var summary = new MessageSummary
                {
                    id = msg.Id.ToString(),
                    status = msg.Status.ToString(),
                    sentat = msg.SentAt.ToString(),
                    sourceinstanceid = msg.Id.SourceInstanceId.ToString(),
                    headers = msg.Headers.ToDictionary()
                };

                if (msg is PersistentMessageToSend)
                {
                    summary.destination = msg.As<PersistentMessageToSend>().Endpoint.ToString();
                }


                return summary;
            }).ToArray();

            return new QueueMessagesVisualization
            {
                Port = input.Port,
                QueueName = input.QueueName,
                Messages = messages
            };
        }

        public QueueMessageVisualization get_message_Port_QueueName_SourceInstanceId_MessageId(
            MessageInputModel input)
        {
            var messageId = new MessageId
            {
                MessageIdentifier = input.MessageId,
                SourceInstanceId = input.SourceInstanceId
            };

            var message = retrieveMessage(messageId, input.Port, input.QueueName);

            if (message == null)
            {
                return new QueueMessageVisualization {NotFound = true};
            }

            var envelope = message.ToEnvelope();
            envelope.UseSerializer(_serializer);

            // TODO -- show errors if in error queue

            var model = new QueueMessageVisualization
            {
                MessageId = messageId,
                QueueName = message.Queue,
                SubQueueName = message.SubQueue,
                Status = message.Status,
                SentAt = message.SentAt,
                Headers = message.Headers.ToDictionary(),
                Port = input.Port
                
            };

            try
            {
                // TODO -- gotta watch how big this monster would be
                model.Payload = JsonSerialization.ToJson(envelope.Message, true);
            }
            catch (Exception)
            {
                model.Payload = "Could not render as JSON";
            }

            return model;
        }


        private PersistentMessage retrieveMessage(MessageId messageId, int port, string queueName)
        {
            var request = new QueueMessageRetrievalRequest
            {
                Port = port,
                QueueName = queueName,
                MessageId = new MessageId
                {
                    MessageIdentifier = messageId.MessageIdentifier,
                    SourceInstanceId = messageId.SourceInstanceId
                }
            };

            return _queueMessageRetrieval.GetSingleMessageInQueue(request);
        }
    }

    public static class NameValueCollectionExtensions
    {
        public static Dictionary<string, string> ToDictionary(this NameValueCollection collection)
        {
            var dict = new Dictionary<string, string>();
            collection.AllKeys.Each(key =>
            {
                dict.Add(key, collection[key]);
            });

            return dict;
        }
    }

    public class MessageSummary
    {
        public MessageSummary()
        {
            destination = "n/a";
        }

        public string id { get; set; }
        public string status { get; set; }
        public string sentat { get; set; }
        public string sourceinstanceid { get; set; }
        public IDictionary<string, string> headers { get; set; }
        public string destination { get; set; }
    }

    public class QueueMessagesVisualization
    {
        public string QueueName { get; set; }
        public MessageSummary[] Messages { get; set; }
        public int Port { get; set; }
    }

    public class MessagesInputModel
    {
        public int Port { get; set; }
        public string QueueName { get; set; }
    }

    public class QueueDto
    {
        public string QueueName { get; set; }
        public int Port { get; set; }
        public int NumberOfMessages { get; set; }
    }

    public class QueueManagersVisualization
    {
        public IList<QueueManagerModel> QueueManagers { get; set; }
    }
}