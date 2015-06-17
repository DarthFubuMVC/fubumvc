using System;
using System.Collections.Generic;
using System.Linq;
using FubuMVC.Core.Urls;
using HtmlTags;
using LightningQueues.Model;

namespace FubuTransportation.LightningQueues.Diagnostics
{
    public class MessagesTableTag : TableTag
    {
        private readonly IUrlRegistry _urls;

        public MessagesTableTag(IEnumerable<QueueMessage> messages, IUrlRegistry urls)
            : this(messages, urls, null, null) {}

        protected MessagesTableTag(IEnumerable<QueueMessage> messages, IUrlRegistry urls,
            Action<TableRowTag> additionalHeaders, Action<TableRowTag, PersistentMessage> additionalColumns)
        {
            _urls = urls;
            AddClass("table");

            AddHeaderRow(row =>
            {
                row.Header("Id");
                row.Header("Status");
                row.Header("Sent At");
                row.Header("Headers");
                if (additionalHeaders != null) additionalHeaders(row);
            });

            messages.Each(message => AddBodyRow(row =>
            {
                AddMessageRow(row, message);
                if (additionalColumns != null) additionalColumns(row, message.InternalMessage);
            }));
        }

        protected void AddMessageRow(TableRowTag row, QueueMessage queueMessage)
        {
            var message = queueMessage.InternalMessage;
            var url = BuildUrlForMessage(queueMessage);

            row.Cell().Add("a").Attr("href", url).Text(message.Id.ToString());
            row.Cell(message.Status.ToString());
            row.Cell(message.SentAt.ToString());
            var list = new HtmlTag("dl", row.Cell());
            foreach (var key in message.Headers.AllKeys)
            {
                var label = new HtmlTag("dt").Text(key + ":");
                var value = new HtmlTag("dd").Text(message.Headers[key]);
                list.Append(label).Append(value);
            }
        }

        protected string BuildUrlForMessage(QueueMessage queueMessage)
        {
            var message = queueMessage.InternalMessage;
            var inputModel = InputModelBuilders.First(x => x.CanHandle(message)).ConstructInputModel();

            inputModel.MessageId = message.Id.MessageIdentifier;
            inputModel.SourceInstanceId = message.Id.SourceInstanceId;
            inputModel.Port = queueMessage.PortNumber;
            inputModel.QueueName = queueMessage.OriginalQueueName;

            return _urls.UrlFor(inputModel);
        }

        private static readonly IList<IQueueMessageInputModelBuilder> InputModelBuilders =
            new List<IQueueMessageInputModelBuilder>
            {
                new QueueMessageInputModelBuilder<ErrorMessageInputModel>
                    { CanHandle = msg => msg.Queue == LightningQueuesTransport.ErrorQueueName },
                new QueueMessageInputModelBuilder<MessageInputModel>
                    { CanHandle = _ => true }
            };

        private class QueueMessageInputModelBuilder<T> : IQueueMessageInputModelBuilder where T : MessageInputModel, new()
        {
            public Func<PersistentMessage, bool> CanHandle { get; set; }

            public MessageInputModel ConstructInputModel()
            {
                return new T();
            }
        }

        private interface IQueueMessageInputModelBuilder
        {
            Func<PersistentMessage, bool> CanHandle { get; set; }
            MessageInputModel ConstructInputModel();
        }
    }

    public class SendingMessagesTableTag : MessagesTableTag
    {
        private static readonly Action<TableRowTag> AdditionalHeader =
            row => row.Header("Destination");
        private static readonly Action<TableRowTag, PersistentMessage> AdditionalColumn =
            (row, message) => row.Cell((message as PersistentMessageToSend).Endpoint.ToString());

        public SendingMessagesTableTag(IEnumerable<QueueMessage> messages, IUrlRegistry urls)
            : base(messages, urls, AdditionalHeader, AdditionalColumn) {}
    }

    public class QueueMessage
    {
        public PersistentMessage InternalMessage { get; set; }
        public string OriginalQueueName { get; set; }
        public int PortNumber { get; set; }
    }
}
