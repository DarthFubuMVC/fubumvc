using System;
using System.Threading;

namespace FubuMVC.Core.Services.Messaging
{
    public class MessageWaitCondition<T> : IListener<T>
    {
        private readonly Func<T, bool> _filter;
        private T _received;
        private readonly ManualResetEvent _reset;

        public MessageWaitCondition(Func<T, bool> filter)
        {
            _filter = filter;

            _reset = new ManualResetEvent(false);
        }

        public T Wait(int wait = 5000)
        {
            _reset.WaitOne(wait);

            return _received;
        }

        public T Received
        {
            get { return _received; }
        }

        public void Receive(T message)
        {
            if (_filter(message))
            {
                _received = message;
                _reset.Set();
            }
        }
    }
}