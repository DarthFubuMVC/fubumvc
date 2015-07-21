using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using FubuCore;
using FubuMVC.Core.ServiceBus.ErrorHandling;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Invocation;

namespace FubuMVC.Core.ServiceBus.InMemory
{
    public class InMemoryQueue : IDisposable
    {
        private readonly Uri _uri;
        private BlockingCollection<byte[]> _queue = InitializeQueue();
        private readonly BinaryFormatter _formatter;
        private bool _disposed;

        public InMemoryQueue(Uri uri)
        {
            _uri = uri;
            _formatter = new BinaryFormatter();
        }

        public Uri Uri
        {
            get { return _uri; }
        }

        public void Enqueue(EnvelopeToken envelope)
        {
            using (var stream = new MemoryStream())
            {
                _formatter.Serialize(stream, envelope);

                stream.Position = 0;
                var bytes = stream.ReadAllBytes();

                _queue.Add(bytes);
            }
        }

        public IEnumerable<EnvelopeToken> Peek()
        {
            return _queue.ToArray().Select(x => _formatter.Deserialize(new MemoryStream(x)).As<EnvelopeToken>());
        }

        public void Clear()
        {
            var oldQueue = _queue;
            _queue = InitializeQueue();
            oldQueue.CompleteAdding();
        }

        public void Dispose()
        {
            _disposed = true;
            _queue.CompleteAdding();
        }

        public void EnsureReady()
        {
            // InMemoryQueues are shared across multiple channels, so they might need to be reset.
            _disposed = false;
            if (_queue.IsAddingCompleted)
            {
                _queue = InitializeQueue();
            }
        }

        public void Receive(IReceiver receiver)
        {
            while (!_disposed)
            {
                foreach (var data in _queue.GetConsumingEnumerable())
                {
                    using (var stream = new MemoryStream(data))
                    {
                        var token = _formatter.Deserialize(stream).As<EnvelopeToken>();

                        var callback = new InMemoryCallback(this, token);

                        receiver.Receive(token.Data, token.Headers, callback);
                    }
                }
            }
        }

        private static BlockingCollection<byte[]> InitializeQueue()
        {
            return new BlockingCollection<byte[]>(new ConcurrentBag<byte[]>());
        }
    }

    public class InMemoryCallback : IMessageCallback
    {
        private readonly InMemoryQueue _parent;
        private readonly EnvelopeToken _token;

        public InMemoryCallback(InMemoryQueue parent, EnvelopeToken token)
        {
            _parent = parent;
            _token = token;
        }

        public void MarkSuccessful()
        {
            // nothing
        }

        public void MarkFailed(Exception ex)
        {
            Debug.WriteLine("Message was marked as failed!");
        }

        public void MoveToDelayedUntil(DateTime time)
        {
            //TODO leverage delayed message cache?
            _token.ExecutionTime = time;
            InMemoryQueueManager.AddToDelayedQueue(_token);
        }

        // TODO -- make it fancier later and copy envelope headers
        public void MoveToErrors(ErrorReport report)
        {
            var uri = (_parent.Uri.ToString() + "errors").ToUri();
            InMemoryQueueManager.QueueFor(uri).Enqueue(new EnvelopeToken
            {
                Message = report
            });
        }

        public void Requeue()
        {
            _parent.Enqueue(_token);
        }
    }
}