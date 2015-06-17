using System.Linq;
using FubuMVC.Core.Urls;
using HtmlTags;

namespace FubuTransportation.LightningQueues.Diagnostics
{
    public class MessagesFubuDiagnostics
    {
        private readonly IQueueMessageRetrieval _queueMessageRetrieval;
        private readonly IUrlRegistry _urls;

        public MessagesFubuDiagnostics(IQueueMessageRetrieval queueMessageRetrieval, IUrlRegistry urls)
        {
            _queueMessageRetrieval = queueMessageRetrieval;
            _urls = urls;
        }

        public QueueMessagesVisualization get_messages_details_Port_QueueName(MessagesInputModel input)
        {
            var request = new QueueMessageRetrievalRequest
            {
                Port = input.Port,
                QueueName = input.QueueName
            };

            var messages = _queueMessageRetrieval.GetAllMessagesInQueue(request).Select(msg => new QueueMessage
            {
                InternalMessage = msg,
                OriginalQueueName = input.QueueName,
                PortNumber = input.Port
            });
            var outgoing = input.QueueName == "outgoing" || input.QueueName == "outgoing_history";

            return new QueueMessagesVisualization
            {
                QueueName = input.QueueName,
                Messages = outgoing
                    ? new SendingMessagesTableTag(messages, _urls)
                    : new MessagesTableTag(messages, _urls)
            };
        }
    }

    public class QueueMessagesVisualization
    {
        public string QueueName { get; set; }
        public TableTag Messages { get; set; }
    }

    public class MessagesInputModel
    {
        public int Port { get; set; }
        public string QueueName { get; set; }
    }

}