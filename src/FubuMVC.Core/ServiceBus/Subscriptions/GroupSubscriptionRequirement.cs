using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.ServiceBus.Configuration;

namespace FubuMVC.Core.ServiceBus.Subscriptions
{
    public class GroupSubscriptionRequirement<T> : ISubscriptionRequirement<T>
    {
        private readonly Accessor _source;
        private readonly Accessor _receiver;
        private readonly IList<Type> _messageTypes = new List<Type>(); 

        public GroupSubscriptionRequirement(Expression<Func<T, Uri>> sourceProperty, Expression<Func<T, Uri>> receiverProperty)
        {
            _source = ReflectionHelper.GetAccessor(sourceProperty);
            _receiver = ReflectionHelper.GetAccessor(receiverProperty);
        }

        public IEnumerable<Subscription> Determine(T settings, ChannelGraph graph)
        {
            var source = _source.GetValue(settings).As<Uri>();
            if (source == null) throw new InvalidOperationException("No Uri established for {0}.{1}".ToFormat(typeof(T).Name, _source.Name));

            var receiver = _receiver.GetValue(settings).As<Uri>();
            if (receiver == null) throw new InvalidOperationException("No Uri established for {0}.{1}".ToFormat(typeof(T).Name, _receiver.Name));

            foreach (var messageType in _messageTypes)
            {
                yield return new Subscription(messageType)
                {
                    NodeName = graph.Name,
                    Receiver = receiver,
                    Source = source
                };
            }


        }

        public void AddType(Type type)
        {
            _messageTypes.Add(type);
        }
    }
}