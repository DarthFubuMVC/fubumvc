using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FubuCore;
using FubuCore.Reflection;
using FubuMVC.Core.ServiceBus.Configuration;

namespace FubuMVC.Core.ServiceBus.Subscriptions
{
    public class LocalSubscriptionRequirement<T> : ISubscriptionRequirement<T>
    {
        private readonly IList<Type> _messageTypes = new List<Type>();
        private readonly Accessor _accessor;

        public LocalSubscriptionRequirement(Expression<Func<T, Uri>> sourceProperty)
        {
            _accessor = ReflectionHelper.GetAccessor(sourceProperty);
        }

        public IEnumerable<Subscription> Determine(T settings, ChannelGraph graph)
        {
            var source = _accessor.GetValue(settings).As<Uri>();
            if (source == null) throw new InvalidOperationException("No Uri established for {0}.{1}".ToFormat(typeof(T).Name, _accessor.Name));

            var receiver = graph.ReplyChannelFor(source.Scheme);

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