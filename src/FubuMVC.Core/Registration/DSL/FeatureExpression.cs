using System;
using System.Linq;
using FubuMVC.Core.Localization;
using FubuMVC.Core.Security.AntiForgery;
using FubuMVC.Core.Security.Authentication;
using FubuMVC.Core.ServerSentEvents;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Runtime.Serializers;
using FubuMVC.Core.ServiceBus.Sagas;
using FubuMVC.Core.Validation.Web;

namespace FubuMVC.Core.Registration.DSL
{
    public class FeatureExpression
    {
        private readonly FubuRegistry _parent;

        public FeatureExpression(FubuRegistry parent)
        {
            _parent = parent;
        }

        /// <summary>
        /// Configure the onboard diagnostics behavior
        /// </summary>
        public Feature<DiagnosticsSettings, TraceLevel> Diagnostics
        {
            get
            {
                return new Feature<DiagnosticsSettings, TraceLevel>(_parent, (settings, level) => settings.TraceLevel = level);
            }
        }

        /// <summary>
        /// Configure and enable the built in authentication features
        /// </summary>
        public Feature<AuthenticationSettings, bool> Authentication
        {
            get
            {
                return new Feature<AuthenticationSettings, bool>(_parent, (settings, enabled) => settings.Enabled = enabled);
            }
        }

        /// <summary>
        /// Configure and enable the built in localization features
        /// </summary>
        public Feature<LocalizationSettings, bool> Localization
        {
            get
            {
                return new Feature<LocalizationSettings, bool>(_parent, (settings, enabled) => settings.Enabled = enabled);
            }
        }

        /// <summary>
        /// Configure and enable the built in anti-forgery functionality
        /// </summary>
        public Feature<AntiForgerySettings, bool> AntiForgery
        {
            get
            {
                return new Feature<AntiForgerySettings, bool>(_parent, (settings, enabled) => settings.Enabled = enabled);
            }
        }

        /// <summary>
        /// Enable the Server Sent Events feature
        /// </summary>
        public Feature<ServerSentEventsSettings, bool> ServerSentEvents
        {
            get
            {
                return new Feature<ServerSentEventsSettings, bool>(_parent, (x, enabled) => x.Enabled = enabled);
            }
        }

        public Feature<ValidationSettings, bool> Validation
        {
            get
            {
                return new Feature<ValidationSettings, bool>(_parent, (x, enabled) => x.Enabled = enabled);
            }
        }

    }

    public class ServiceBusFeature : Feature<TransportSettings, bool>
    {
        public ServiceBusFeature(FubuRegistry parent)
            : base(parent, (settings, enabled) => settings.Enabled = enabled)
        {
        }

        /// <summary>
        /// Enable the in memory transport
        /// </summary>
        public void EnableInMemoryTransport(Uri replyUri = null)
        {
            Configure(x =>
            {
                x.InMemoryTransport = InMemoryTransportMode.Enabled;
                if (replyUri != null)
                {
                    x.InMemoryReplyUri = replyUri;
                }
            });
        }

        public void SagaStorage<T>() where T : ISagaStorage, new()
        {
            Configure(x => x.SagaStorageProviders.Add(new T()));
        }

        public void DefaultSerializer<T>() where T : IMessageSerializer, new()
        {
            _parent.AlterSettings<ChannelGraph>(graph => graph.DefaultContentType = new T().ContentType);
        }

        public void DefaultContentType(string contentType)
        {
            _parent.AlterSettings<ChannelGraph>(graph => graph.DefaultContentType = contentType);
        }

        public void AcceptedContentTypes(params IMessageSerializer[] serializers)
        {
            _parent.AlterSettings<ChannelGraph>(graph =>
                graph.AcceptedContentTypes = graph.AcceptedContentTypes
                    .Concat(serializers.Select(x => x.ContentType)));
        }

        public void AcceptedContentTypes(params string[] contentTypes)
        {
            _parent.AlterSettings<ChannelGraph>(graph =>
                graph.AcceptedContentTypes = graph.AcceptedContentTypes.Concat(contentTypes));
        }

        public HealthMonitoringExpression HealthMonitoring
        {
            get
            {
                return new HealthMonitoringExpression(_parent);
            }
        }


    }
}
