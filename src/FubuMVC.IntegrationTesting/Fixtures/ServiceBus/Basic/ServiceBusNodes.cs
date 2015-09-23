using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Diagnostics;
using Serenity.ServiceBus;
using StoryTeller;
using StructureMap;
using TestMessages.ScenarioSupport;
using MessageHistory = FubuMVC.Core.Services.Messaging.Tracking.MessageHistory;

namespace FubuMVC.IntegrationTesting.Fixtures.ServiceBus.Basic
{
    public class ServiceBusNodes
    {
        public static int Port = 2480;

        private static readonly Type[] _messageTypes = typeof (Message).Assembly.GetExportedTypes()
            .Where(x => x.CanBeCastTo<Message>() && x != typeof (Message))
            .ToArray();

        private readonly Cache<string, FubuRuntime> _runtimes = new Cache<string, FubuRuntime>();

        public static readonly IEnumerable<PropertyInfo> Channels
            = typeof (HarnessSettings).GetProperties().Where(x => x.PropertyType == typeof (Uri)).ToArray();

        private readonly HarnessSettings _settings;

        public ServiceBusNodes()
        {
            _settings = new HarnessSettings();

            Channels.Each(prop =>
            {
                Port++;

                var name = prop.Name;
                var uri = new Uri("lq.tcp://localhost:" + Port + "/" + name.ToLower());
                prop.SetValue(_settings, uri);
            });
        }

        public static Type[] MessageTypes
        {
            get { return _messageTypes; }
        }

        public static Type FindMessageType(string message)
        {
            return _messageTypes.First(x => x.Name == message);
        }

        public void ClearAll()
        {
            MessageHistory.ClearAll();
            _runtimes.Each(x => x.SafeDispose());
            _runtimes.ClearAll();
        }


        public void Send(string key, string node, string messageName)
        {
            var runtime = _runtimes[node];
            var messageType = FindMessageType(messageName);

            var sender =
                runtime.Get<IContainer>()
                    .ForGenericType(typeof (Sender<>))
                    .WithParameters(messageType)
                    .GetInstanceAs<ISender>();


            sender.SendNew(key);
        }

        public void AddTracing(ISpecContext context)
        {
            _runtimes.Each((name, runtime) =>
            {
                var provider = new MessageContextualInfoProvider(runtime.Get<IMessagingSession>())
                {
                    ShortTitle = name,
                    Title = name + " Messaging"
                };
                context.Reporting.Log(provider);
            });
        }

        public NodeRegistry CreateNew(string node)
        {
            return new NodeRegistry(node, _settings);
        }

        public void Start(NodeRegistry current)
        {
            var runtime = current.ToRuntime();
            _runtimes[current.NodeName] = runtime;
        }
    }

    public interface ISender
    {
        void SendNew(string key);
    }

    public class Sender<T> : ISender where T : Message, new()
    {
        private readonly IServiceBus _bus;

        public Sender(IServiceBus bus)
        {
            _bus = bus;
        }

        public void SendNew(string key)
        {
            var message = new T
            {
                Key = key
            };
            _bus.Send(message);
        }
    }
}