using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core;
using FubuMVC.Core.Registration;
using FubuMVC.Core.ServiceBus;
using FubuMVC.Core.ServiceBus.Configuration;
using FubuMVC.Core.ServiceBus.Events;
using FubuMVC.Core.ServiceBus.InMemory;
using FubuMVC.Core.ServiceBus.Scheduling;
using FubuMVC.Core.ServiceBus.TestSupport;
using StructureMap;

namespace FubuMVC.Tests.ServiceBus.ScenarioSupport
{
    public class NodeConfiguration : IDisposable
    {
        private readonly Expression<Func<HarnessSettings, Uri>> _expression;
        private readonly Lazy<FubuTransportRegistry<HarnessSettings>> _registry = new Lazy<FubuTransportRegistry<HarnessSettings>>(() => FubuTransportRegistry<HarnessSettings>.Empty()); 
        private FubuRuntime _runtime;
        private IServiceBus _serviceBus;
        private Uri _uri;
        private readonly IList<string> _info = new List<string>(); 

        public NodeConfiguration(Expression<Func<HarnessSettings, Uri>> expression)
        {
            _expression = expression;
        }

        internal Scenario Parent { get; set; }

        internal IServiceBus ServiceBus
        {
            get { return _serviceBus; }
        }

        internal void SpinUp()
        {
            if (!_registry.IsValueCreated) return;

            var registry = _registry.Value;
            var nodeName = ReflectionHelper.GetProperty(_expression).Name;
            registry.NodeName = nodeName;


            registry.Channel(_expression).ReadIncoming(new ThreadScheduler(2));

            registry.Handlers.DisableDefaultHandlerSource();
            registry.Handlers.Include<SourceRecordingHandler>();
            registry.AlterSettings<TransportSettings>(x => x.DebugEnabled = true);

            var container = new Container();
            registry.StructureMap(container);

            // Make it all be 
            var harnessSettings = InMemoryTransport.ToInMemory<HarnessSettings>();
            container.Configure(x => {
                x.For<HarnessSettings>().Use(harnessSettings);
                x.For<IListener>().Add<MessageWatcher>();
                //x.For<ILogListener>().Add(new ScenarioLogListener(nodeName));
            });

            _uri = (Uri) ReflectionHelper.GetAccessor(_expression).GetValue(harnessSettings);

            _runtime = FubuApplication.For(registry).Bootstrap();
            _serviceBus = container.GetInstance<IServiceBus>();
        }

        public SendExpression<T> Sends<T>(string description) where T : Message, new()
        {
            return new SendExpression<T>(Parent, this, description);
        }

        public void SendAndAwait<T>() where T : Message, new()
        {
            var step = new SendAndAwaitStep<T>(this);
            Parent.AddStep(step);
        }

        public class SendExpression<T> where T : Message, new()
        {
            private readonly Scenario _parent;
            private readonly NodeConfiguration _sender;
            private readonly string _description;
            private readonly SendMessageStep<T> _step;

            public SendExpression(Scenario parent, NodeConfiguration sender, string description)
            {
                _parent = parent;
                _sender = sender;
                _description = description;
                _step = new SendMessageStep<T>(sender, description);
                parent.AddStep(_step);


            }

            public SendExpression<T> ShouldBeReceivedBy(NodeConfiguration node)
            {
                _step.ReceivingNodes.Add(node);
                return this;
            }

            public SendExpression<T> MatchingMessageIsReceivedBy<TMatching>(NodeConfiguration receiver) where TMatching : Message, new()
            {
                _parent.AddStep(new MatchingMessageStep<TMatching>(_step, receiver));
                return this;
            }
        }

        public HandlesExpresion<T> Handles<T>() where T : Message
        {
            _registry.Value.Handlers.Include<SimpleHandler<T>>();
        
            return new HandlesExpresion<T>(this);
        }

        public class HandlesExpresion<T> where T : Message
        {
            private readonly NodeConfiguration _parent;

            public HandlesExpresion(NodeConfiguration parent)
            {
                parent._registry.Value.Handlers.Include<SimpleHandler<T>>();
                _parent = parent;
            }

            public HandlesExpresion<T> Raises<TResponse>() where TResponse : Message, new()
            {
                _parent._info.Add("Handling {0} raises {1}".ToFormat(typeof(T).Name, typeof(TResponse).Name));
                _parent._registry.Value.Handlers.Include<RequestResponseHandler<T, TResponse>>();
                return this;
            } 
        }

        public ReplyExpression<T> Requesting<T>() where T : Message
        {
            return new ReplyExpression<T>(this);
        }

        public FubuTransportRegistry<HarnessSettings> Registry
        {
            get { return _registry.Value; }
        }

        public class ReplyExpression<T> where T : Message
        {
            private readonly NodeConfiguration _parent;

            public ReplyExpression(NodeConfiguration parent)
            {
                _parent = parent;
            }

            public ReplyExpression<T> RespondWith<TResponse>() where TResponse : Message, new()
            {
                _parent._registry.Value.Handlers.Include<RequestResponseHandler<T, TResponse>>();
                return this;
            } 
        }

        public string Name
        {
            get
            {
                return ReflectionHelper.GetAccessor(_expression).Name;
            }
        }

        public Uri Uri
        {
            get { return _uri; }
        }

        internal void Describe(IScenarioWriter writer)
        {
            if (!_registry.IsValueCreated) return;

            writer.WriteLine(Name);
            var channels = _runtime.Factory.Get<ChannelGraph>();
            using (writer.Indent())
            {
                channels.Where(x => x.Incoming)
                        .Each(x => writer.WriteLine("Listens to {0} with ", x.Uri, x.Scheduler));
            
                //writer.BlankLine();

                channels.Where(x => x.Rules.Any()).Each(x => {
                    writer.WriteLine("Publishes to {0}", x.Key);

                    using (writer.Indent())
                    {
                        x.Rules.Each(rule => writer.Bullet(rule.Describe()));
                    }
                });

                var handlers = _runtime.Factory.Get<BehaviorGraph>();
                var inputs = handlers.Handlers.Select(x => x.InputType()).Where(x => x != typeof(object[]));
                if (inputs.Any())
                {
                    writer.WriteLine("Handles " + inputs.Select(x => x.Name).Join(", "));
                }

                _info.Each(x => writer.WriteLine(x));

                writer.BlankLine();
            }
        }


        void IDisposable.Dispose()
        {
            if (_runtime != null)
            {
                _runtime.Dispose();
            }
        }

        public IReplyExpectation Requests<T>(string description) where T : Message, new()
        {
            return new RequestReplyExpression<T>(this, this.Parent, description);
        }

        internal bool Received(Message message)
        {
            return TestMessageRecorder.AllProcessed.Any(processed => {
                return processed.Message.GetType() == message.GetType() && processed.Message.Id == message.Id &&
                       processed.ReceivedAt == _uri;
            });
        }
    }
}