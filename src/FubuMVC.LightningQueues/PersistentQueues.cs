using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Concurrency;
using System.Runtime.Serialization;
using FubuCore.Util;
using LightningDB;
using LightningQueues;
using FubuCore.Logging;
using LightningQueues.Storage.LMDB;

namespace FubuMVC.LightningQueues
{
    public class PersistentQueues : IPersistentQueues
    {
        private readonly ILogger _logger;
        public string QueuePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fubutransportationqueues");

        private readonly Cache<int, Queue> _queueManagers;

        public PersistentQueues(ILogger logger)
        {
            _logger = logger;
            _queueManagers = new Cache<int, Queue>(port => BuildQueue(new IPEndPoint(IPAddress.Any, port), QueuePath + "." + port));
        }

        private Queue BuildQueue(IPEndPoint endpoint, string queuePath)
        {
            return new QueueConfiguration()
                .ReceiveMessagesAt(endpoint)
                .StoreWithLmdb(queuePath, new EnvironmentConfiguration {MaxDatabases = 5, MapSize = 1024*1024*100}) //TODO pull through settings
                .ScheduleQueueWith(TaskPoolScheduler.Default)
                .LogWith(new FubuLoggingAdapter(_logger))
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