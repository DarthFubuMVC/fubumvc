using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.ServiceBus.Polling;
using FubuMVC.Core.ServiceBus.Runtime;
using HtmlTags;

namespace FubuMVC.Core.ServiceBus.Diagnostics
{
    public class MessageRecord : MessageRecordNode
    {
        private Type _messageType;
        public string Id;
        public string Message;
        public string Node;
        public string ParentId;
        public string Type;
        public string Headers;
        public string ExceptionText;

        public MessageRecord()
        {
        }

        public MessageRecord(EnvelopeToken envelope)
        {
            Id = envelope.CorrelationId;
            ParentId = envelope.ParentId;
            if (envelope.Message != null)
            {
                _messageType = envelope.Message.GetType();
                Type = _messageType.FullName;
            }

            Headers = envelope.Headers.Keys().Select(x => "{0}={1}".ToFormat(x, envelope.Headers[x])).Join(";");
        }

        public bool IsPollingJobRelated()
        {
            return _messageType != null && _messageType.Closes(typeof(JobRequest<>));
        }

        public override HtmlTag ToLeafTag()
        {
            return new HtmlTag("li").Text("{0}: {1}".ToFormat(Node, Message));
        }

        public override string ToString()
        {
            return string.Format("{0} from {2}, Message: {1}, Headers: {3}", Id, Message, Node, Headers);
        }
    }
}