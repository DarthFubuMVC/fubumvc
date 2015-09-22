using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.ServiceBus.InMemory;
using FubuMVC.Core.ServiceBus.Registration;
using FubuMVC.Core.ServiceBus.Registration.Nodes;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Routing;
using FubuMVC.Core.ServiceBus.Runtime.Serializers;
using FubuMVC.Core.ServiceBus.ScheduledJobs.Configuration;
using FubuMVC.Core.ServiceBus.Scheduling;
using FubuMVC.Core.ServiceBus.Subscriptions;

namespace FubuMVC.Core.ServiceBus.Configuration
{

    public class FubuTransportRegistry<T> : FubuRegistry
    {
        protected FubuTransportRegistry()
        {
            ServiceBus.Enable(true);

            AlterSettings<TransportSettings>(x => x.SettingTypes.Fill(typeof (T)));

            // TODO -- what is this? Why are we doing this?
            AlterSettings<ChannelGraph>(graph =>
            {
                if (InMemoryTransport.DefaultSettings == typeof (T))
                {
                    InMemoryTransport.DefaultChannelGraph = graph;
                }
            });
        }

        public ScheduledJobExpression<T> ScheduledJob
        {
            get { return new ScheduledJobExpression<T>(this); }
        }


        public static FubuTransportRegistry<T> Empty()
        {
            return new FubuTransportRegistry<T>();
        }


        public ChannelExpression Channel(Expression<Func<T, Uri>> expression)
        {
            return new ChannelExpression(this, expression);
        }

        public class ChannelExpression
        {
            private readonly FubuTransportRegistry<T> _parent;
            private readonly Accessor _accessor;

            public ChannelExpression(FubuTransportRegistry<T> parent, Expression<Func<T, Uri>> expression)
            {
                _parent = parent;
                _accessor = ReflectionHelper.GetAccessor(expression);
            }

            private Action<ChannelNode> alter
            {
                set
                {
                    _parent.AlterSettings<ChannelGraph>(graph =>
                    {
                        var node = graph.ChannelFor(_accessor);
                        value(node);
                    });
                }
            }

            /// <summary>
            /// Add an IEnvelopeModifier that will apply to only this channel
            /// </summary>
            /// <typeparam name="TModifier"></typeparam>
            /// <returns></returns>
            public ChannelExpression ModifyWith<TModifier>() where TModifier : IEnvelopeModifier, new()
            {
                return ModifyWith(new TModifier());
            }

            /// <summary>
            /// Add an IEnvelopeModifier that will apply to only this channel
            /// </summary>
            /// <param name="modifier"></param>
            /// <returns></returns>
            public ChannelExpression ModifyWith(IEnvelopeModifier modifier)
            {
                alter = node => node.Modifiers.Add(modifier);

                return this;
            }

            public ChannelExpression DefaultSerializer<TSerializer>() where TSerializer : IMessageSerializer, new()
            {
                alter = node => node.DefaultSerializer = new TSerializer();
                return this;
            }

            public ChannelExpression DefaultContentType(string contentType)
            {
                alter = node => node.DefaultContentType = contentType;
                return this;
            }

            public ChannelExpression ReadIncoming(IScheduler scheduler = null)
            {
                alter = node =>
                {
                    var defaultScheduler = node.Scheduler;
                    node.Incoming = true;
                    node.Scheduler = scheduler ?? defaultScheduler;
                };
                return this;
            }

            public ChannelExpression ReadIncoming(SchedulerMaker<T> schedulerMaker)
            {
                alter = node =>
                {
                    node.Incoming = true;
                    node.SettingsRules.Add(schedulerMaker);
                };
                return this;
            }


            public ChannelExpression AcceptsMessagesInNamespaceContainingType<TMessageType>()
            {
                alter = node => node.Rules.Add(NamespaceRule.For<TMessageType>());
                return this;
            }

            public ChannelExpression AcceptsMessagesInNamespace(string @namespace)
            {
                alter = node => node.Rules.Add(new NamespaceRule(@namespace));
                return this;
            }

            public ChannelExpression AcceptsMessagesInAssemblyContainingType<TMessageType>()
            {
                alter = node => node.Rules.Add(AssemblyRule.For<TMessageType>());
                return this;
            }

            public ChannelExpression AcceptsMessagesInAssembly(string assemblyName)
            {
                var assembly = Assembly.Load(assemblyName);

                alter = node => node.Rules.Add(new AssemblyRule(assembly));
                return this;
            }

            public ChannelExpression AcceptsMessage<TMessage>()
            {
                alter = node => node.Rules.Add(new SingleTypeRoutingRule<TMessage>());
                return this;
            }

            public ChannelExpression AcceptsMessage(Type messageType)
            {
                alter =
                    node => node.Rules.Add(typeof (SingleTypeRoutingRule<>).CloseAndBuildAs<IRoutingRule>(messageType));
                return this;
            }

            public ChannelExpression AcceptsMessages(Expression<Func<Type, bool>> filter)
            {
                alter = node => node.Rules.Add(new LambdaRoutingRule(filter));
                return this;
            }

            public ChannelExpression AcceptsMessagesMatchingRule<TRule>() where TRule : IRoutingRule, new()
            {
                alter = node => node.Rules.Add(new TRule());
                return this;
            }
        }

        public ByThreadScheduleMaker<T> ByThreads(Expression<Func<T, int>> property)
        {
            return new ByThreadScheduleMaker<T>(property);
        }

        public ByTaskScheduleMaker<T> ByTasks(Expression<Func<T, int>> property)
        {
            return new ByTaskScheduleMaker<T>(property);
        }

        public SubscriptionExpression SubscribeAt(Expression<Func<T, Uri>> receiving)
        {
            return new SubscriptionExpression(this, receiving);
        }

        public SubscriptionExpression SubscribeLocally()
        {
            return new SubscriptionExpression(this, null);
        }

        public class SubscriptionExpression
        {
            private readonly FubuTransportRegistry<T> _parent;
            private readonly Expression<Func<T, Uri>> _receiving;

            public SubscriptionExpression(FubuTransportRegistry<T> parent, Expression<Func<T, Uri>> receiving)
            {
                _parent = parent;
                _receiving = receiving;

                parent.Services.AddType(typeof (ISubscriptionRequirement), typeof (SubscriptionRequirements<T>));
            }

            /// <summary>
            /// Specify the publishing source of the events you want to subscribe to
            /// </summary>
            /// <param name="sourceProperty"></param>
            /// <returns></returns>
            public TypeSubscriptionExpression ToSource(Expression<Func<T, Uri>> sourceProperty)
            {
                var requirement = _receiving == null
                    ? (ISubscriptionRequirement<T>) new LocalSubscriptionRequirement<T>(sourceProperty)
                    : new GroupSubscriptionRequirement<T>(sourceProperty, _receiving);

                _parent.Services.AddService(requirement);

                return new TypeSubscriptionExpression(requirement);
            }

            public class TypeSubscriptionExpression
            {
                private readonly ISubscriptionRequirement<T> _requirement;

                public TypeSubscriptionExpression(ISubscriptionRequirement<T> requirement)
                {
                    _requirement = requirement;
                }

                public TypeSubscriptionExpression ToMessage<TMessage>()
                {
                    _requirement.AddType(typeof (TMessage));

                    return this;
                }

                public TypeSubscriptionExpression ToMessage(Type messageType)
                {
                    _requirement.AddType(messageType);
                    return this;
                }
            }
        }
    }

    public class NulloHandlerSource : IHandlerSource
    {
        public Task<HandlerCall[]> FindCalls(Assembly applicationAssembly)
        {
            return Task.FromResult(new HandlerCall[0]);
        }
    }
}