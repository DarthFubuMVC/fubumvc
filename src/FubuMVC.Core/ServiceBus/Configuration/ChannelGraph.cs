using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Logging;
using FubuCore.Reflection;
using FubuCore.Util;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using FubuMVC.Core.ServiceBus.Runtime.Serializers;

namespace FubuMVC.Core.ServiceBus.Configuration
{
    public class ChannelGraph : IEnumerable<ChannelNode>, IDisposable
    {
        private readonly Cache<string, ChannelNode> _channels =
            new Cache<string, ChannelNode>(key => new ChannelNode {Key = key});

        private readonly Cache<string, Uri> _replyChannels =
            new Cache<string, Uri>();

        public ChannelGraph()
        {
            DefaultContentType = new XmlMessageSerializer().ContentType;

            _replyChannels.OnMissing = name =>
            {
                var channels = _channels.Where(x => x.Protocol() == name).ToArray();

                var incoming = channels.FirstOrDefault(x => x.Incoming);


                if (incoming != null) return incoming.Uri;

                if (channels.Any())
                {
                    var existing = channels.Select(x => $"{x.Uri} Incoming = {x.Incoming}").Join(", ");
                    throw new ArgumentOutOfRangeException($"No known reply channel for protocol '{name}', existing channels are {existing}");
                }
                else
                {
                    throw new ArgumentOutOfRangeException($"No known channels for protocol '{name}'");
                }
            };
        }

        private string _nodeId;

        public string NodeId
        {
            get
            {
                if (_nodeId.IsEmpty())
                {
                    return Name + "@" + System.Environment.MachineName;
                }

                return _nodeId;
            }
            set
            {
                _nodeId = value;
            }
        }


        /// <summary>
        /// Used to identify the instance of the running FT node
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The default content type to use for serialization if none is specified at
        /// either the message or channel level
        /// </summary>
        public string DefaultContentType { get; set; }

        public List<string> AcceptedContentTypes { get; set; } = new List<string>();

        public bool HasChannels => _channels.Any();

        public ChannelNode ControlChannel { get; set; }

        public ChannelNode ChannelFor<T>(Expression<Func<T, Uri>> property)
        {
            return ChannelFor(ReflectionHelper.GetAccessor(property));
        }

        public ChannelNode ChannelFor(string name)
        {
            return _channels.FirstOrDefault(x => x.SettingAddress.Name == name);
        }

        public ChannelNode ChannelFor(Accessor accessor)
        {
            var key = ToKey(accessor);
            var channel = _channels[key];
            channel.SettingAddress = accessor;

            return channel;
        }

        public IEnumerable<Uri> ReplyUriList()
        {
            return _replyChannels;
        }

        public Uri ReplyChannelFor(string protocol)
        {
            return _replyChannels[protocol];
        }

        public List<string> GetAcceptedContentTypesForChannel(Uri channelUri)
        {
            var acceptedContentTypes = new List<string>();
            var node = this.FirstOrDefault(x => x.Channel.Address == channelUri) ?? this.FirstOrDefault(x => x.Uri == channelUri);

            acceptedContentTypes.AddRange(node?.AcceptedContentTypes ?? Enumerable.Empty<string>());
            acceptedContentTypes.Add(node?.DefaultContentType);
            acceptedContentTypes.Add(node?.DefaultSerializer?.ContentType);
            acceptedContentTypes.AddRange(AcceptedContentTypes);
            acceptedContentTypes.Add(DefaultContentType);

            return acceptedContentTypes.Where(x => x != null).Distinct().ToList();
        }

        public void AddReplyChannel(string protocol, Uri uri)
        {
            _replyChannels[protocol] = uri;
        }

        public IEnumerable<ChannelNode> NodesForProtocol(string protocol)
        {
            return _channels.Where(x => x.Protocol() != null && x.Protocol().EqualsIgnoreCase(protocol))
                .Distinct()
                .ToArray();
        }


        // leave it virtual for testing
        public virtual void ReadSettings(IServiceLocator services)
        {
            _channels.Each(x => x.ReadSettings(services));
        }

        public virtual void StartReceiving(IHandlerPipeline pipeline, ILogger logger)
        {
            _channels.Where(x => x.Incoming).Each(node => node.StartReceiving(pipeline, logger, this));
        }

        public static string ToKey(Accessor accessor)
        {
            return accessor.OwnerType.Name.Replace("Settings", "") + ":" + accessor.Name;
        }

        public static string ToKey<T>(Expression<Func<T, object>> property)
        {
            return ToKey(property.ToAccessor());
        }

        public IEnumerator<ChannelNode> GetEnumerator()
        {
            return _channels.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(ChannelNode replyNode)
        {
            _channels[replyNode.Key] = replyNode;
        }

        private bool _wasDisposed;

        public void Dispose()
        {
            if (_wasDisposed) return;

            _channels.Each(x => x.Dispose());

            _wasDisposed = true;
        }
    }
}
