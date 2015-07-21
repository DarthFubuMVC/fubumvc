using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.ServiceBus.Runtime;

namespace FubuMVC.Core.ServiceBus.InMemory
{

    public static class InMemoryQueueManager
    {
        public static readonly Uri DelayedUri = "memory://localhost/delayed".ToUri();

        private static readonly Cache<Uri, InMemoryQueue> _queues = new Cache<Uri,InMemoryQueue>(x => new InMemoryQueue(x));
        private static readonly IList<EnvelopeToken> _delayed = new List<EnvelopeToken>(); 
        private static readonly ReaderWriterLockSlim _delayedLock = new ReaderWriterLockSlim();
    
        public static void ClearAll()
        {
            _delayedLock.Write(() => {
                _delayed.Clear();
            });

            
            _queues.Each(x => x.Clear());
        }

        public static void Remove(InMemoryQueue queue)
        {
            _queues.Remove(queue.Uri);
        }

        public static void AddToDelayedQueue(EnvelopeToken envelope)
        {
            _delayedLock.Write(() => {
                _delayed.Add(envelope);
            });
        }

        public static IEnumerable<EnvelopeToken> DequeueDelayedEnvelopes(DateTime currentTime)
        {
            var delayed = _delayedLock.Read(() => {
                return _delayed.Where(x => new Envelope(x.Headers).ExecutionTime.Value <= currentTime).ToArray();
            });

            var list = new List<EnvelopeToken>();

            foreach (EnvelopeToken token in delayed)
            {
                _delayedLock.Write(() => {
                    try
                    {
                        _delayed.Remove(token);

                        var envelope = new Envelope(token.Headers);
                        _queues[envelope.ReceivedAt].Enqueue(token);

                        list.Add(token);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                    }
                });

            }

            return list;
        } 

        public static InMemoryQueue QueueFor(Uri uri)
        {
            var queue = _queues[uri];
            queue.EnsureReady();
            return queue;
        }

        public static IEnumerable<EnvelopeToken> DelayedEnvelopes()
        {
            return _delayedLock.Read(() => {
                return _delayed.ToArray();
            });
        }
    }
}