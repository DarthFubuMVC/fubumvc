using System;
using System.Collections.Specialized;
using FubuMVC.Core.Runtime;
using FubuMVC.Core.ServiceBus.ErrorHandling;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Headers;
using FubuMVC.Core.ServiceBus.Runtime.Serializers;
using FubuMVC.Core.Services;
using HtmlTags;
using LightningQueues.Model;
using LightningQueues.Storage;

namespace FubuTransportation.LightningQueues.Diagnostics
{
    public class MessageFubuDiagnostics
    {
        private readonly IQueueMessageRetrieval _queueMessageRetrieval;
        private readonly IEnvelopeSerializer _serializer;
        private readonly IFubuRequest _fubuRequest;

        public MessageFubuDiagnostics(IQueueMessageRetrieval queueMessageRetrieval, IEnvelopeSerializer serializer,
            IFubuRequest fubuRequest)
        {
            _queueMessageRetrieval = queueMessageRetrieval;
            _serializer = serializer;
            _fubuRequest = fubuRequest;
        }

        [QueueMessageResourceNotFound]
        public QueueMessageVisualization get_message_details_Port_QueueName_SourceInstanceId_MessageId(
            MessageInputModel input)
        {
            var messageId = new MessageId
            {
                MessageIdentifier = input.MessageId,
                SourceInstanceId = input.SourceInstanceId
            };
            var message = RetrieveMessage(messageId, input.Port, input.QueueName);

            if (message == null)
            {
                _fubuRequest.Set(new QueueMessageNotFound
                {
                    Id = messageId,
                    QueueName = input.QueueName
                });

                return null;
            }

            var envelope = message.ToEnvelope();
            envelope.UseSerializer(_serializer);

            return new QueueMessageVisualization
            {
                MessageId = messageId,
                QueueName = message.Queue,
                SubQueueName = message.SubQueue,
                Status = message.Status,
                SentAt = message.SentAt,
                Headers = message.Headers,
                Payload = envelope.Message
            };
        }

        [QueueMessageResourceNotFound]
        public ErrorQueueMessageVisualization get_error_message_details_Port_QueueName_SourceInstanceId_MessageId(
            ErrorMessageInputModel input)
        {
            var messageId = new MessageId
            {
                MessageIdentifier = input.MessageId,
                SourceInstanceId = input.SourceInstanceId
            };
            var message = RetrieveMessage(messageId, input.Port, input.QueueName);

            if (message == null)
            {
                _fubuRequest.Set(new QueueMessageNotFound
                {
                    Id = messageId,
                    QueueName = input.QueueName
                });

                return null;
            }

            var errorReport = ErrorReport.Deserialize(message.Data);
            var exceptionDetails = new ExceptionDetails
            {
                Explanation = errorReport.Explanation,
                ExceptionType = errorReport.ExceptionType,
                ExceptionMessage = errorReport.ExceptionMessage,
                ExceptionText = errorReport.ExceptionText
            };

            var envelope = new Envelope(new NameValueHeaders(message.Headers)) {Data = errorReport.RawData};
            envelope.UseSerializer(_serializer);

            return new ErrorQueueMessageVisualization
            {
                MessageId = messageId,
                QueueName = message.Queue,
                SubQueueName = message.SubQueue,
                Status = message.Status,
                SentAt = message.SentAt,
                Headers = message.Headers,
                Payload = envelope.Message,
                ExceptionDetails = exceptionDetails
            };
        }

        private PersistentMessage RetrieveMessage(MessageId messageId, int port, string queueName)
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

    public class QueueMessageVisualization
    {
        public MessageId MessageId { get; set; }
        public string QueueName { get; set; }
        public string SubQueueName { get; set; }
        public MessageStatus Status { get; set; }
        public DateTime SentAt { get; set; }
        public NameValueCollection Headers { get; set; }
        public object Payload { get; set; }

        public string PayloadAsJson
        {
            get { return Payload != null ? JsonSerialization.ToJson(Payload, true) : null; }
        }

        public HtmlTag HeadersAsHtml
        {
            get
            {
                var list = new HtmlTag("dl");
                foreach (var key in Headers.AllKeys)
                {
                    var label = new HtmlTag("dt").Text(key + ":");
                    var value = new HtmlTag("dd").Text(Headers[key]);
                    list.Append(label).Append(value);
                }

                return list;
            }
        }
    }

    public class ErrorQueueMessageVisualization : QueueMessageVisualization
    {
        public ExceptionDetails ExceptionDetails { get; set; }
    }

    public class ExceptionDetails
    {
        public string Explanation { get; set; }
        public string ExceptionType { get; set; }
        public string ExceptionMessage { get; set; }
        public string ExceptionText { get; set; }
    }

    public class MessageInputModel
    {
        public Guid MessageId { get; set; }
        public Guid SourceInstanceId { get; set; }
        public int Port { get; set; }
        public string QueueName { get; set; }
    }

    public class ErrorMessageInputModel : MessageInputModel
    {
    }
}