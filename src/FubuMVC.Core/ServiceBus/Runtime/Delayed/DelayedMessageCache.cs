using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FubuCore;
using StructureMap;

namespace FubuMVC.Core.ServiceBus.Runtime.Delayed
{
    public interface IDelayedMessageCache<TIdentifier>
    {
        void Add(TIdentifier messageId, DateTime time);
        IEnumerable<TIdentifier> AllMessagesBefore(DateTime time);
    }

    [Singleton]
    public class DelayedMessageCache<TIdentifier> : IDelayedMessageCache<TIdentifier>
    {
        private readonly IList<DelayedMessage> _delayedMessages = new List<DelayedMessage>();
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        public void Add(TIdentifier messageId, DateTime time)
        {
            _lock.Write(() => _delayedMessages.Add(new DelayedMessage{MessageId = messageId, ReceiveAt = time}));
        }
        
        public IEnumerable<TIdentifier> AllMessagesBefore(DateTime time)
        {
            var readyToPlayMessages = _lock.Read<IList<DelayedMessage>>(() => 
                _delayedMessages.Where(x => x.ReceiveAt <= time).ToArray());

            if (readyToPlayMessages.Any())
            {
                _lock.Write(() => readyToPlayMessages.Each(x => _delayedMessages.Remove(x)));
            }

            return readyToPlayMessages.Select(x => x.MessageId).ToArray();
        }

        private class DelayedMessage
        {
            public DateTime ReceiveAt;
            public TIdentifier MessageId;
        }
    }
}