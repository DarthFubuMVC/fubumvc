using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Concurrency;
using System.Runtime.Serialization;
using FubuCore.Util;
using LightningDB;
using FubuCore.Logging;
using FubuMVC.LightningQueues.Queues;
using FubuMVC.LightningQueues.Queues.Persistence;
using FubuMVC.LightningQueues.Queues.Storage;

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
            _queueManagers = new Cache<int, Queue>();
        }

        private Queue GetQueue(int port, bool persist, bool incoming, int mapSize = 1024*1024*100, int maxDatabases = 5)
        {
            if (!incoming)
            {
                //Shouldn't create one here because it shouldn't be listening
                return _queueManagers.First();
            }
            if (_queueManagers.Has(port))
            {
                return _queueManagers[port];
            }
            return CreateQueue(port, persist, mapSize, maxDatabases);
        }

        private Queue CreateQueue(int port, bool persist, int mapSize = 1024*1024*100, int maxDatabases = 5)
        {
            var queueConfiguration = new QueueConfiguration()
                .ReceiveMessagesAt(new IPEndPoint(IPAddress.Any, port))
                .ScheduleQueueWith(TaskPoolScheduler.Default)
                .LogWith(new FubuLoggingAdapter(_logger));

            if (persist)
            {
                queueConfiguration.StoreWithLmdb(QueuePath + "." + port, new EnvironmentConfiguration { MaxDatabases = maxDatabases, MapSize = mapSize });
            }
            else
            {
                queueConfiguration.UseNoStorage();
            }
            var queue = queueConfiguration.BuildQueue();
            _queueManagers.Fill(port, queue);
            return queue;
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

        public Queue PersistentManagerFor(int port, bool incoming, int mapSize = 1024*1024*100, int maxDatabases = 5)
        {
            return GetQueue(port, true, incoming, mapSize, maxDatabases);
        }

        public Queue NonPersistentManagerFor(int port, bool incoming)
        {
            return GetQueue(port, false, incoming);
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

                    var queueManager = GetQueue(@group.Key, true, true);
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
            GetQueue(uri.Port, true, true).CreateQueue(uri.QueueName);
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