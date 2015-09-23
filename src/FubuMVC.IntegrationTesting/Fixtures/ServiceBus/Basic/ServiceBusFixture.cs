using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FubuCore;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using StoryTeller;
using StoryTeller.Grammars.Sets;
using TestMessages.ScenarioSupport;

namespace FubuMVC.IntegrationTesting.Fixtures.ServiceBus.Basic
{
    public class ServiceBusFixture : Fixture
    {

        private ServiceBusNodes _nodes;


        public ServiceBusFixture()
        {
            Title = "Basic Service Bus";
        }

        public override void SetUp()
        {
            TestMessageRecorder.Clear();
            _nodes = new ServiceBusNodes();
            Context.State.Store(_nodes);
        }

        public override void TearDown()
        {
            _nodes.AddTracing(Context);
            _nodes.ClearAll();
        }

        public IGrammar ActiveNode()
        {
            return Embed<ServiceBusNodeFixture>("Active Node");
        }

        public IGrammar Actions()
        {
            return Embed<ServiceBusActionFixture>("Send messages");
        }

        public IGrammar MessagesProcessedShouldBe()
        {
            Action<ObjectComparison<ProcessedMessage>> configure = _ =>
            {
                _.Compare(x => x.Key);
                _.Compare(x => x.Node).SelectionList("Channels");
                _.Compare(x => x.Type).SelectionList("Messages");
                //_.Compare(x => x.Handler).DefaultValue("None");
            };

            return VerifySetOf(processed).Titled("The messages processed should be").MatchOn(configure);
        }

        private IEnumerable<ProcessedMessage> processed()
        {


            return TestMessageRecorder.AllProcessed.Select(x =>
            {
                var name = x.ReceivedAt.ToString().Split('/').Last();

                return new ProcessedMessage
                {
                    Key = x.Message.Key,
                    Node = name.Capitalize(),
                    Type = x.Message.GetType().Name,
                    Handler = x.Description
                };
            }).Distinct();
        }
    }

    public class ProcessedMessage
    {
        public string Key { get; set; }
        public string Type { get; set; }
        public string Node { get; set; }
        public string Handler { get; set; }

        protected bool Equals(ProcessedMessage other)
        {
            return string.Equals(Key, other.Key) && string.Equals(Type, other.Type) && string.Equals(Node, other.Node);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ProcessedMessage) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Key != null ? Key.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Type != null ? Type.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Node != null ? Node.GetHashCode() : 0);
                return hashCode;
            }
        }
    }

    public class NodeRegistry : FubuTransportRegistry<HarnessSettings>
    {
        public NodeRegistry(string node, HarnessSettings settings)
        {
            ReplaceSettings(settings);

            NodeId = NodeName = node;
            Mode = "testing";

            Handlers.FindBy(source =>
            {
                source.UseAssembly(typeof(Message).Assembly);
            });


        }

        public void AddReply(Type messageType, Type replyType)
        {
            var handlerType = typeof (RequestResponseHandler<,>)
                .MakeGenericType(messageType, replyType);

            Handlers.Include(handlerType);
        }
    }


}