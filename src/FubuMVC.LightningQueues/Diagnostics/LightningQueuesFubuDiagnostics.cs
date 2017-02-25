using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.ServiceBus.ErrorHandling;
using FubuMVC.Core.ServiceBus.Runtime.Serializers;
using FubuMVC.Core.Services;
using LightningQueues;

namespace FubuMVC.LightningQueues.Diagnostics
{
    public class LightningQueuesFubuDiagnostics
    {
        private readonly IPersistentQueues _queues;
        private readonly IQueueMessageRetrieval _queueMessageRetrieval;
        private readonly IEnvelopeSerializer _serializer;

        public LightningQueuesFubuDiagnostics(IPersistentQueues queues, IQueueMessageRetrieval queueMessageRetrieval,
            IEnvelopeSerializer serializer)
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
                    sentat = msg.SentAt.ToString(),
                    sourceinstanceid = msg.Id.SourceInstanceId.ToString(),
                    headers = msg.Headers
                };

                if (msg is OutgoingMessage)
                {
                    summary.destination = msg.As<OutgoingMessage>().Destination.ToString();
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

        public QueueMessageVisualization get_message_Port_QueueName_SourceInstanceId_MessageId(MessageInputModel input)
        {
            var messageId = MessageId.Parse($"{input.SourceInstanceId}/{input.MessageId}");

            var message = retrieveMessage(messageId, input.Port, input.QueueName);

            if (message == null)
            {
                return new QueueMessageVisualization {NotFound = true};
            }

            var model = new QueueMessageVisualization
            {
                MessageId = messageId,
                QueueName = message.Queue,
                SentAt = message.SentAt,
                Headers = message.Headers,
                Port = input.Port
            };
            try
            {
                object payload;
                var envelope = message.ToEnvelope();
                envelope.UseSerializer(_serializer, null);
                if (input.QueueName == "errors")
                {
                    var errorReport = ErrorReport.Deserialize(message.Data);
                    message = new Message
                    {
                        Data = errorReport.RawData,
                        Headers = errorReport.Headers,
                        Id = messageId,
                        Queue = input.QueueName,
                    };
                    envelope = message.ToEnvelope();
                    var originalMessage = _serializer.Deserialize(envelope, null);
                    var errorSummary = new ErrorSummary
                    {
                        exceptiontype = errorReport.ExceptionType,
                        exceptionmessage = errorReport.ExceptionMessage,
                        exceptiontext = errorReport.ExceptionText,
                        originalmessage = originalMessage
                    };
                    payload = errorSummary;
                }
                else
                {
                    payload = envelope.Message;
                }


                model.Payload = JsonSerialization.ToJson(payload, true);
            }
            catch (Exception)
            {
                model.Payload = "Could not render as JSON";
            }

            return model;
        }


        private Message retrieveMessage(MessageId messageId, int port, string queueName)
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

    public class ErrorSummary
    {
        public string exceptiontype { get; set; }
        public string exceptionmessage { get; set; }
        public string exceptiontext { get; set; }
        public object originalmessage { get; set; }
    }

    public class MessageSummary
    {
        public MessageSummary()
        {
            destination = "n/a";
        }

        public string id { get; set; }
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
