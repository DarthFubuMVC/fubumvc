using System;
using System.Collections.Generic;

namespace FubuMVC.Core.ServerSentEvents
{
    public interface ITopicChannelCache
    {
        ITopicChannel<T> ChannelFor<T>(T topic) where T : Topic;
        bool TryGetChannelFor<T>(T topic, out ITopicChannel<T> channel) where T : Topic;
        void ClearAll();
        bool IsDisposed { get; }

        void SpinUpTopics<T>(Func<IEnumerable<T>> topics) where T : Topic;
    }
}