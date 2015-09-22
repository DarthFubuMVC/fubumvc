using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using FubuCore.Logging;
using FubuCore.Util;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Delayed;
using LightningQueues;
using LightningQueues.Model;

namespace FubuMVC.LightningQueues
{
    public class PersistentQueues : IPersistentQueues
    {
        private readonly ILogger _logger;
        private readonly IDelayedMessageCache<MessageId> _delayedMessages;
        private readonly QueueManagerConfiguration _queueManagerConfiguration;
        public const string EsentPath = "fubutransportation.esent";

        private readonly Cache<int, QueueManager> _queueManagers;

        public PersistentQueues(ILogger logger, IDelayedMessageCache<MessageId> delayedMessages, LightningQueueSettings settings)
        {
            _logger = logger;
            _delayedMessages = delayedMessages;
            _queueManagerConfiguration = settings.ToConfiguration();
            _queueManagers = new Cache<int, QueueManager>(port => new QueueManager(new IPEndPoint(IPAddress.Any, port), EsentPath + "." + port, _queueManagerConfiguration));
        }

        public void Dispose()
        {
            _queueManagers.Each(x => x.Dispose());
        }

        public IEnumerable<IQueueManager> AllQueueManagers { get { return _queueManagers.GetAll(); } }

        public void ClearAll()
        {
            _queueManagers.Each(x => x.ClearAllMessages());
        }

        public IQueueManager ManagerFor(int port, bool incoming)
        {
            if (incoming)
            {
                return _queueManagers[port];
            }



            return _queueManagers.Any() ? _queueManagers.First() : _queueManagers[port];
        }

        public IQueueManager ManagerForReply()
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
                    queueManager.CreateQueues(queueNames);
                    queueManager.CreateQueues(LightningQueuesTransport.DelayedQueueName);
                    queueManager.CreateQueues(LightningQueuesTransport.ErrorQueueName);

                    queueManager.Start();
                    RecoverDelayedMessages(queueManager);
                }
                catch (Exception e)
                {
                    throw new LightningQueueTransportException(new IPEndPoint(IPAddress.Any, group.Key), e);
                }
            });
        }

        private void RecoverDelayedMessages(QueueManager queueManager)
        {
            queueManager.GetQueue(LightningQueuesTransport.DelayedQueueName)
                .GetAllMessages(null)
                .Each(x => _delayedMessages.Add(x.Id, x.ExecutionTime()));
        }

        public void CreateQueue(LightningUri uri)
        {
            _queueManagers[uri.Port].CreateQueues(uri.QueueName);
        }

        public IEnumerable<EnvelopeToken> ReplayDelayed(DateTime currentTime)
        {
            return _queueManagers.SelectMany(x => ReplayDelayed(x, currentTime));
        }

        public IEnumerable<EnvelopeToken> ReplayDelayed(QueueManager queueManager, DateTime currentTime)
        {
            var list = new List<EnvelopeToken>();

            var transactionalScope = queueManager.BeginTransactionalScope();
            try
            {
                var readyToSend = _delayedMessages.AllMessagesBefore(currentTime);

                readyToSend.Each(x =>
                {
                    var message = transactionalScope.ReceiveById(LightningQueuesTransport.DelayedQueueName, x);
                    var uri = message.Headers[Envelope.ReceivedAtKey].ToLightningUri();
                    MessagePayload messagePayload = message.ToPayload();
                    transactionalScope.EnqueueDirectlyTo(uri.QueueName, messagePayload);
                    list.Add(message.ToToken());
                });
                transactionalScope.Commit();
            }

            catch (Exception e)
            {
                transactionalScope.Rollback();
                _logger.Error("Error trying to move delayed messages back to the original queue", e);
            }

            return list;
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