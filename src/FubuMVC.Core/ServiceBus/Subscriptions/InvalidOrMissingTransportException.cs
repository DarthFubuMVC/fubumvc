using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using FubuCore;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Runtime;

namespace FubuMVC.Core.ServiceBus.Subscriptions
{
    [Serializable]
    public class InvalidOrMissingTransportException : Exception
    {
        public static string ToMessage(IEnumerable<ITransport> transports, IEnumerable<ChannelNode> nodes)
        {
            return "Missing channel Uri configuration or unknown transport types\nAvailable transports are " +
                   transports.Select(x => x.ToString()).Join(", ") + " and the invalid nodes are \n" +
                   nodes.Select(x => { return "Node '{0}'@{1}; ".ToFormat(x.Key, x.Uri); }).Join("\n");
        }

        public static string ToMessage(Exception ex, IEnumerable<ITransport> transports, IEnumerable<ChannelNode> nodes)
        {
            return ex.Message + "\nAvailable transports are " + transports.Select(x => x.ToString()).Join(", ") +
                   " and the nodes are \n" +
                   nodes.Select(x => { return "Node '{0}'@{1}, Incoming={2}; ".ToFormat(x.Key, x.Uri, x.Incoming); })
                       .Join("\n");
        }

        public InvalidOrMissingTransportException(Exception ex, IEnumerable<ITransport> transports,
            IEnumerable<ChannelNode> nodes)
            : base(ToMessage(ex, transports, nodes), ex)
        {
        }

        public InvalidOrMissingTransportException(IEnumerable<ITransport> transports, IEnumerable<ChannelNode> nodes)
            : base(ToMessage(transports, nodes))
        {
        }

        protected InvalidOrMissingTransportException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}