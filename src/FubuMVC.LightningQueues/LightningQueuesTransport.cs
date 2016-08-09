using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Runtime;

namespace FubuMVC.LightningQueues
{
    public class LightningQueuesTransport : TransportBase, ITransport
    {
        private const string StaticLibraryName = "lmdb.dll";
        private static readonly bool _exported = false;

        static LightningQueuesTransport()
        {
            if (!_exported)
            {
                try
                {


                    var shouldWrite = true;
                    var folder = Environment.Is64BitProcess ? "x64" : "x86";
                    var assembly = Assembly.GetExecutingAssembly();
                    byte[] resourceAssemblyBytes;

                    string resourceName = $"FubuMVC.LightningQueues.{folder}.{StaticLibraryName}";

                    var stream = assembly.GetManifestResourceStream(resourceName);
                    using (var memoryStream = new MemoryStream())
                    {
                        stream.CopyTo(memoryStream);
                        resourceAssemblyBytes = memoryStream.GetBuffer();
                    }

                    var path = AppDomain.CurrentDomain.BaseDirectory.AppendPath(StaticLibraryName);
                    if (File.Exists(path))
                    {
                        var existingBytes = File.ReadAllBytes(path);
                        shouldWrite = !resourceAssemblyBytes.SequenceEqual(existingBytes);
                    }
                    if (shouldWrite)
                    {
                        File.WriteAllBytes(path, resourceAssemblyBytes);
                        Console.WriteLine($"Successfully wrote resource {resourceName} to {path}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unable to write the lmdb.dll file-->\n\n" + e);
                }
                finally
                {
                    _exported = true;
                }
            }
        }


        public static readonly string ErrorQueueName = "errors";

        private readonly IPersistentQueues _queues;
        private readonly LightningQueueSettings _settings;

        public LightningQueuesTransport(IPersistentQueues queues, LightningQueueSettings settings)
        {
            _queues = queues;
            _settings = settings;
        }

        public void Dispose()
        {
            // IPersistentQueues is disposable
        }

        public override string Protocol => LightningUri.Protocol;

        public override bool Disabled(IEnumerable<ChannelNode> nodes)
        {
            if (_settings.Disabled) return true;

            if (!nodes.Any() && _settings.DisableIfNoChannels) return true;

            return false;
        }

        public IChannel BuildDestinationChannel(Uri destination)
        {
            var lqUri = new LightningUri(destination);
            return new LightningQueuesReplyChannel(destination, _queues.ManagerForReply(), lqUri.QueueName);
        }

        public IEnumerable<EnvelopeToken> ReplayDelayed(DateTime currentTime)
        {
            return Enumerable.Empty<EnvelopeToken>();
        }

        public void ClearAll()
        {
            _queues.ClearAll();
        }

        protected override IChannel buildChannel(ChannelNode channelNode)
        {
            var uri = new LightningUri(channelNode.Uri);
            return channelNode.Mode == ChannelMode.DeliveryGuaranteed
                ? LightningQueuesChannel.BuildPersistentChannel(uri, _queues, _settings.MapSize, _settings.MaxDatabases, channelNode.Incoming)
                : LightningQueuesChannel.BuildNoPersistenceChannel(uri, _queues, channelNode.Incoming);
        }


        protected override void seedQueues(IEnumerable<ChannelNode> channels)
        {
            var groups = channels.GroupBy(x => x.Uri.Port).Where(x => x.Count() > 1);
            foreach (var group in groups)
            {
                if (group.Select(x => x.Mode).Distinct().Count() > 1)
                {
                    throw new InvalidOperationException($"The LightningQueues listener for port {group.Key} has mixed channel modes. Either make the modes be the same or use a different port number");
                }
            }


            _queues.Start(channels.Where(x => x.Incoming).Select(x => new LightningUri(x.Uri)));
        }

        protected override Uri getReplyUri(ChannelGraph graph)
        {
            var channelNode = graph.FirstOrDefault(x => x.Protocol() == LightningUri.Protocol && x.Incoming && x.Mode == ChannelMode.DeliveryGuaranteed)
                ?? graph.FirstOrDefault(x => x.Protocol() == LightningUri.Protocol && x.Incoming && x.Mode == ChannelMode.DeliveryFastWithoutGuarantee);
            if (channelNode == null)
                throw new InvalidOperationException("You must have at least one incoming Lightning Queue channel for accepting replies");

            return channelNode.Channel.Address.ToLocalUri();
        }
    }
}