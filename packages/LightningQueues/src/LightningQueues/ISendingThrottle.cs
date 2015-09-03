using System;
using System.Threading;

namespace LightningQueues
{
    public interface ISendingThrottle
    {
        int CurrentlySendingCount { get; }
        int CurrentlyConnectingCount { get; }
        int MaxSendingCount { get; }
        int MaxConnectingCount { get; }
        int AvailableSendingCount { get; }
        void AlterSendingCountMaximumTo(int maxSendingCount);
        void AlterConnectingCountMaximumTo(int maxConnectingCount);
    }

    internal interface ISendingGovernor
    {
        bool ShouldBeginSend();
        void NoMessagesToSend();
        void SuccessfullyConnected();
        void StartSend();
        void FailedToConnect();
        void FinishedSend();
        void StopSending();
    }

    public class SendingChoke : ISendingThrottle, ISendingGovernor
    {
        private readonly object _sendingLock = new object();
        private int _maxSendingCount;
        private int _maxConnectingCount;
        private int _currentlySendingCount;
        private int _currentlyConnectingCount;

        public SendingChoke()
        {
            _maxConnectingCount = 30;
            _maxSendingCount = 5;
        }

        public int CurrentlySendingCount
        {
            get
            {
                var currentlySending = 0;
                Interlocked.Exchange(ref currentlySending, _currentlySendingCount);
                return currentlySending;
            }
        }

        public int CurrentlyConnectingCount
        {
            get
            {
                var currentlyConnecting = 0;
                Interlocked.Exchange(ref currentlyConnecting, _currentlyConnectingCount);
                return currentlyConnecting;
            }
        }

        public int MaxSendingCount { get { return _maxSendingCount; } }
        public int MaxConnectingCount { get { return _maxConnectingCount; } }

        public int AvailableSendingCount
        {
            get
            {
                if (CurrentlyConnectingCount >= MaxConnectingCount)
                    return 0;
                return MaxSendingCount - (CurrentlySendingCount - CurrentlyConnectingCount);
            }
        }

        public void AlterSendingCountMaximumTo(int maxSendingCount)
        {
            Interlocked.Exchange(ref _maxSendingCount, maxSendingCount);
        }

        public void AlterConnectingCountMaximumTo(int maxConnectingCount)
        {
            Interlocked.Exchange(ref _maxConnectingCount, maxConnectingCount);
        }

        public bool ShouldBeginSend()
        {
            //normal conditions will be at max sending count, when there are several unreliable endpoints 
            //it will grow up to max connecting count attempting to connect, timeouts can take up to 30 seconds
            if ((_currentlySendingCount - _currentlyConnectingCount < _maxSendingCount) &&
                _currentlyConnectingCount <= _maxConnectingCount) return true;
            lock (_sendingLock)
                Monitor.Wait(_sendingLock, TimeSpan.FromSeconds(1));
            return false;
        }

        public void NoMessagesToSend()
        {
            lock (_sendingLock)
                Monitor.Wait(_sendingLock, TimeSpan.FromSeconds(1));
        }

        public void SuccessfullyConnected()
        {
            Interlocked.Decrement(ref _currentlyConnectingCount);
        }

        public void StartSend()
        {
            Interlocked.Increment(ref _currentlySendingCount);
            Interlocked.Increment(ref _currentlyConnectingCount);
        }

        public void FailedToConnect()
        {
            Interlocked.Decrement(ref _currentlyConnectingCount);
        }

        public void FinishedSend()
        {
            Interlocked.Decrement(ref _currentlySendingCount);
        }

        public void StopSending()
        {
            while (_currentlySendingCount > 0)
                Thread.Sleep(TimeSpan.FromSeconds(1));
			lock(_sendingLock)
				Monitor.Pulse(_sendingLock);
        }
    }

}