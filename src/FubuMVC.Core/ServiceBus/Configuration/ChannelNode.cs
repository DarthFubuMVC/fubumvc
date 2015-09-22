using System;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuCore.Logging;
using FubuCore.Reflection;
using FubuMVC.Core.ServiceBus.Logging;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Headers;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;
using FubuMVC.Core.ServiceBus.Runtime.Routing;
using FubuMVC.Core.ServiceBus.Runtime.Serializers;
using FubuMVC.Core.ServiceBus.Scheduling;

namespace FubuMVC.Core.ServiceBus.Configuration
{
    public class ChannelNode : IDisposable
    {
        public Accessor SettingAddress
        {
            get { return _settingAddress; }
            set
            {
                if (value.PropertyType != typeof (Uri))
                {
                    throw new ArgumentOutOfRangeException("SettingAddress", "Can only be a Uri property");
                }
                _settingAddress = value;
            }
        }

        public readonly IList<IEnvelopeModifier> Modifiers = new List<IEnvelopeModifier>(); 

        public readonly IList<ISettingsAware> SettingsRules = new List<ISettingsAware>(); 

        public string Key { get; set; }

        public IScheduler Scheduler = TaskScheduler.Default();
        public bool Incoming = false;

        public IList<IRoutingRule> Rules = new List<IRoutingRule>();
        private Accessor _settingAddress;

        public Uri Uri { get; set; }
        public IChannel Channel { get; set; }

        

        public bool Publishes(Type type)
        {
            return Rules.Any(x => x.Matches(type));
        }
        
        public void ReadSettings(IServiceLocator services)
        {
            var settings = services.GetInstance(SettingAddress.OwnerType);
            Uri = (Uri) SettingAddress.GetValue(settings);

            SettingsRules.Each(x => x.ApplySettings(settings, this));
        }

        public string Protocol()
        {
            return Uri != null ? Uri.Scheme : null;
        }

        public override string ToString()
        {
            return string.Format("Channel: {0}", Key);
        }

        public void Dispose()
        {
            // TODO -- going to come back and try to make the scheduler "drain"
            Channel.Dispose();
            Scheduler.Dispose();
        }

        public void StartReceiving(IHandlerPipeline pipeline, ILogger logger, ChannelGraph graph)
        {
            if (Channel == null) throw new InvalidOperationException("Cannot receive on node {0} without a matching channel".ToFormat(SettingAddress));
            var receiver = new Receiver(pipeline, graph, this);
            StartReceiving(receiver, logger);
        }

        public void StartReceiving(IReceiver receiver, ILogger logger)
        {
            Scheduler.Start(() =>
            {
                int exceptionCount = 0;
                var receivingState = ReceivingState.CanContinueReceiving;
                while (receivingState == ReceivingState.CanContinueReceiving)
                {
                    try
                    {
                        receivingState = Channel.Receive(receiver);
                        exceptionCount = 0;
                    }
                    catch (Exception ex)
                    {
                        logger.InfoMessage(new ReceiveFailed { ChannelKey = Key, Exception = ex });
                        logger.Error("Error in receive loop", ex);
                        if (++exceptionCount > 9)
                        {
                            // We're probably stuck getting the same error forever, let the process crash.
                            throw new ReceiveFailureException(
                                "Received repeated errors while listening for messages on channel: " + Key,
                                ex);
                        }
                    }
                }
            });
        }

        // virtual for testing of course
        public virtual IHeaders Send(Envelope envelope, IEnvelopeSerializer serializer, Uri replyUri = null)
        {
            var clone = envelope.Clone();

            // Must be done in this order!
            Modifiers.Each(x => x.Modify(clone));
            serializer.Serialize(clone, this);

            clone.Headers[Envelope.DestinationKey] = Uri.ToString();
            clone.Headers[Envelope.ChannelKey] = Key;

            if (replyUri != null)
            {
                clone.Headers[Envelope.ReplyUriKey] = replyUri.ToString();
            }

            Channel.Send(clone.Data, clone.Headers);

            return clone.Headers;
        }

        private string _defaultContentType;
        private IMessageSerializer _defaultSerializer;

        public IMessageSerializer DefaultSerializer
        {
            get { return _defaultSerializer; }
            set
            {
                _defaultContentType = null;
                _defaultSerializer = value;
            }
        }

        public string DefaultContentType
        {
            get
            {
                return _defaultContentType;
            }
            set
            {
                _defaultSerializer = null;
                _defaultContentType = value;
            }
        }
    }

    
}