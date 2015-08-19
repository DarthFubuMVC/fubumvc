using System;
using System.Collections.Generic;
using System.Threading;
using FubuCore;
using FubuCore.Util;

namespace FubuMVC.Core.ServerSentEvents
{
    public class TopicChannelCache : ITopicChannelCache, IDisposable
    {
        private readonly IAspNetShutDownDetector _shutdownDetector;
        private readonly Cache<Type, ITopicFamily> _families;
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);

        public bool IsDisposed { get; private set; }

        public TopicChannelCache(IServiceLocator services, IAspNetShutDownDetector shutdownDetector)
        {
            _families = new Cache<Type, ITopicFamily>(type =>
            {
                var familyType = typeof (TopicFamily<>).MakeGenericType(type);
                return (ITopicFamily) services.GetInstance(familyType);
            });

            _shutdownDetector = shutdownDetector;
            _shutdownDetector.Register(DisposeAction);
        }

        public ITopicChannel<T> ChannelFor<T>(T topic) where T : Topic
        {
            return _lock.Read(() =>
            {
                if (IsDisposed)
                    throw new ObjectDisposedException("TopicChannelCache");

                return _families[typeof (T)].As<TopicFamily<T>>().ChannelFor(topic);
            });
        }

        public bool TryGetChannelFor<T>(T topic, out ITopicChannel<T> channel) where T : Topic
        {
            try
            {
                channel = ChannelFor(topic);
                return true;
            }
            catch(Exception)
            {
                channel = null;
                return false;
            }
        }

        public void ClearAll()
        {
            _lock.Write(() =>
            {
                _families.Each(x => x.Flush());

                _families.ClearAll();
            });
        }

        public void SpinUpTopics<T>(Func<IEnumerable<T>> topics) where T : Topic
        {
            _lock.Write(() =>
            {
                if (IsDisposed)
                    throw new ObjectDisposedException("TopicChannelCache");

                var family = _families[typeof (T)].As<TopicFamily<T>>();
                topics().Each(family.SpinUpChannel);
            });
        }

        public void Dispose()
        {
            _shutdownDetector.Dispose();
            DisposeAction();
        }

        private void DisposeAction()
        {
            _lock.Write(() =>
            {
                IsDisposed = true;
                ClearAll();
            });
        }
    }
}