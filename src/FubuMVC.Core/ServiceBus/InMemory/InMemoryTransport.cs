using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FubuCore;
using FubuCore.Descriptions;
using FubuCore.Reflection;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Runtime;

namespace FubuMVC.Core.ServiceBus.InMemory
{
    [Description("A simple in memory transport suitable for automated testing or development")]
    [Title("In Memory Transport")]
    public class InMemoryTransport : TransportBase, ITransport
    {
        internal static Type DefaultSettings { get; set; }
        internal static ChannelGraph DefaultChannelGraph { get; set; }

        private readonly TransportSettings _settings;

        public InMemoryTransport(TransportSettings settings)
        {
            _settings = settings;
        }

        public InMemoryTransport() : this(new TransportSettings())
        {
        }

        public void Dispose()
        {
            // nothing
        }

        public override string Protocol
        {
            get { return InMemoryChannel.Protocol; }
        }

        public IChannel BuildDestinationChannel(Uri destination)
        {
            return new InMemoryChannel(destination);
        }

        public IEnumerable<EnvelopeToken> ReplayDelayed(DateTime currentTime)
        {
            return InMemoryQueueManager.DequeueDelayedEnvelopes(currentTime);
        }

        public void ClearAll()
        {
            InMemoryQueueManager.ClearAll();
        }

        protected override IChannel buildChannel(ChannelNode channelNode)
        {
            return new InMemoryChannel(channelNode.Uri);
        }

        protected override void seedQueues(IEnumerable<ChannelNode> channels)
        {

        }

        protected override Uri getReplyUri(ChannelGraph graph)
        {
            var uri = _settings.InMemoryReplyUri ?? ReplyUriForGraph(graph);
            var replyNode = new ChannelNode
            {
                Uri = uri,
                Incoming = true
            };

            replyNode.Key = replyNode.Key ?? "{0}:{1}".ToFormat(Protocol, "replies");
            replyNode.Channel = buildChannel(replyNode);

            graph.Add(replyNode);

            return uri;
        }

        public static Uri ReplyUriForGraph(ChannelGraph graph)
        {
            return "{0}://localhost/{1}/replies".ToFormat(InMemoryChannel.Protocol, graph.Name ?? "node").ToUri();
        }

        public static T ToInMemory<T>() where T : new()
        {
            var type = typeof (T);
            var settings = ToInMemory(type);

            return (T) settings;
        }

        public static object ToInMemory(Type type)
        {
            var settings = Activator.CreateInstance(type);

            AllChannelsAreInMemory(type, settings);

            return settings;
        }

        public static void AllChannelsAreInMemory(Type type, object settings)
        {
            type.GetProperties().Where(x => x.CanWrite && x.PropertyType == typeof (Uri)).Each(prop =>
            {
                var accessor = new SingleProperty(prop);
                var uri = GetUriForProperty(accessor);

                accessor.SetValue(settings, uri);
            });
        }

        private static Uri GetUriForProperty(SingleProperty accessor)
        {
            var channelGraph = DefaultChannelGraph;
            if (channelGraph != null && accessor.DeclaringType != DefaultSettings)
            {
                // A default graph has been set via SetupForInMemoryTesting, so
                // sync the URIs for the channels that match.
                var channel = Enumerable.FirstOrDefault<ChannelNode>(channelGraph, x =>
                {
                    string channelName = x.Key.Split(':')[1];
                    return channelName == accessor.Name;
                });

                if (channel != null)
                    return channel.Uri;
            }

            string uri = "{0}://{1}/{2}".ToFormat(InMemoryChannel.Protocol, accessor.OwnerType.Name.Replace("Settings", ""),
                accessor.Name).ToLower();
            return new Uri(uri);
        }
    }
}