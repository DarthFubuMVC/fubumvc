using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using FubuMVC.Core.ServiceBus.Events;
using FubuMVC.Core.ServiceBus.Logging;

namespace FubuMVC.Core.ServiceBus.Runtime
{
    public class ReplyListener<T> : IListener<EnvelopeReceived>,IExpiringListener
    {
        private readonly IEventAggregator _events;
        private readonly TaskCompletionSource<T> _completion;
        private readonly string _originalId;
        private Task _timeout;

        public ReplyListener(IEventAggregator events, string originalId, TimeSpan timeout)
        {
            _events = events;
            _completion = new TaskCompletionSource<T>();
            ExpiresAt = DateTime.UtcNow.Add(timeout);
            _originalId = originalId;

            var listener = this;

            _timeout = Task.Delay(timeout).ContinueWith(t => {
                if (!_completion.Task.IsCompleted && !_completion.Task.IsFaulted)
                {
                    _completion.SetException(new TimeoutException());
                }
            });

            _completion.Task.ContinueWith(x => _events.RemoveListener(listener));
        }

        public Task<T> Completion
        {
            get { return _completion.Task; }
        }

        public void Handle(EnvelopeReceived message)
        {
            if (Matches(message.Envelope))
            {
                _completion.SetResult((T) message.Envelope.Message);
                _events.RemoveListener(this);

                IsExpired = true;
            }

            var ack = message.Envelope.Message as FailureAcknowledgement;
            if (ack != null && ack.CorrelationId == _originalId)
            {
                _completion.SetException(new ReplyFailureException(ack.Message));
                _events.RemoveListener(this);

                IsExpired = true;
            }
        }

        public bool Matches(EnvelopeToken envelope)
        {
            return envelope.Message is T && envelope.ResponseId == _originalId;
        }

        protected bool Equals(ReplyListener<T> other)
        {
            return string.Equals(_originalId, other._originalId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ReplyListener<T>) obj);
        }

        public override int GetHashCode()
        {
            return (_originalId != null ? _originalId.GetHashCode() : 0);
        }

        public override string ToString()
        {
            return string.Format("Reply watcher for {0} with Id {1}",typeof(T).FullName, _originalId);
        }

        public bool IsExpired { get; private set; }
        public DateTime? ExpiresAt { get; private set; }
    }

    [Serializable]
    public class ReplyFailureException : Exception
    {
        public ReplyFailureException(string message) : base(message)
        {
        }

        protected ReplyFailureException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}