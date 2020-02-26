using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using LightningQueues;
using LightningQueues.Storage.LMDB;

namespace FubuMVC.LightningQueues.Diagnostics
{
    public class QueueManagerModel
    {
        public QueueManagerModel(Queue queueManager)
        {
            var lmdbStore = queueManager.Store as LmdbMessageStore;
            Path = lmdbStore == null ? "No path" : lmdbStore.Environment.Path;
            Port = queueManager.Endpoint.Port;
            Queues = buildQueues(queueManager).ToArray();
        }

        public int Port { get; set; }
        public string Path { get; set; }
        public QueueDto[] Queues { get; set; }


        private IEnumerable<QueueDto> buildQueues(Queue queues)
        {
            foreach (var queue in queues.Queues)
            {
                yield return new QueueDto
                {
                    Port = queues.Endpoint.Port,
                    QueueName = queue,
                    NumberOfMessages = queues.Store.PersistedMessages(queue).ToEnumerable().Count()
                };
            }
            yield return new QueueDto
            {
                Port = queues.Endpoint.Port,
                QueueName = "outgoing",
                NumberOfMessages = queues.Store.PersistedOutgoingMessages().ToEnumerable().Count()
            };
        }
    }
}
