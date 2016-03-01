using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Concurrency;
using System.Runtime.Serialization;
using FubuCore;
using FubuCore.Util;
using LightningQueues;
using LightningQueues.Logging;
using LightningQueues.Storage.LMDB;

namespace FubuMVC.LightningQueues
{

    public class ConsoleLogger : ILogger
    {
        public void Debug(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        public void DebugFormat(string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine(message.ToFormat(args));
        }

        public void Debug<TMessage>(TMessage message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        public void Info(string message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        public void InfoFormat(string message, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine(message.ToFormat(args));
        }

        public void Info<TMessage>(TMessage message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        public void Error(string message, Exception exception)
        {
            System.Diagnostics.Debug.WriteLine(message);
            System.Diagnostics.Debug.WriteLine(exception);
        }

        public void ErrorFormat(string message, Exception ex, params object[] args)
        {
            System.Diagnostics.Debug.WriteLine(message.ToFormat(args));
            System.Diagnostics.Debug.WriteLine(ex);
        }
    }

    public class PersistentQueues : IPersistentQueues
    {
        public string QueuePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fubutransportationqueues");

        private readonly Cache<int, Queue> _queueManagers;

        public PersistentQueues()
        {
            _queueManagers = new Cache<int, Queue>(port => BuildQueue(new IPEndPoint(IPAddress.Any, port), QueuePath + "-" + port));
        }

        private Queue BuildQueue(IPEndPoint endpoint, string queuePath)
        {
            return new QueueConfiguration()
                .ReceiveMessagesAt(endpoint)
                .StoreWithLmdb(queuePath)
                .LogWith(new NulloLogger()) //todo better logger for queues
                .ScheduleQueueWith(TaskPoolScheduler.Default)
                .LogWith(new ConsoleLogger())
                .BuildQueue();
        }

        public void Dispose()
        {
            _queueManagers.Each(x => x.Dispose());
        }

        public IEnumerable<Queue> AllQueueManagers => _queueManagers.GetAll();

        public void ClearAll()
        {
            _queueManagers.Each(x => x.Store.ClearAllStorage());
        }

        public Queue ManagerFor(int port, bool incoming)
        {
            if (incoming)
            {
                return _queueManagers[port];
            }



            return _queueManagers.Any() ? _queueManagers.First() : _queueManagers[port];
        }

        public Queue ManagerForReply()
        {
            return _queueManagers.First();
        }

        public void Start(IEnumerable<LightningUri> uriList)
        {
            uriList.GroupBy(x => x.Port).Each(group =>
            {
                try
                {
                    string[] queueNames = group.Select(x => x.QueueName).ToArray();

                    var queueManager = _queueManagers[@group.Key];
                    queueNames.Each(x => queueManager.CreateQueue(x));
                    queueManager.CreateQueue(LightningQueuesTransport.ErrorQueueName);
                    queueManager.Start();
                }
                catch (Exception e)
                {
                    throw new LightningQueueTransportException(new IPEndPoint(IPAddress.Any, group.Key), e);
                }
            });
        }

        public void CreateQueue(LightningUri uri)
        {
            _queueManagers[uri.Port].CreateQueue(uri.QueueName);
        }
    }

    [Serializable]
    public class LightningQueueTransportException : Exception
    {
        public LightningQueueTransportException(IPEndPoint endpoint, Exception innerException) : base("Error trying to initialize LightningQueues queue manager at " + endpoint, innerException)
        {
        }

        protected LightningQueueTransportException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}