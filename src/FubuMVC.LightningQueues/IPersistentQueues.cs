using System;
using System.Collections.Generic;
using LightningQueues;

namespace FubuMVC.LightningQueues
{
    public interface IPersistentQueues : IDisposable
    {
        IEnumerable<Queue> AllQueueManagers { get; }
        void ClearAll();
        Queue PersistentManagerFor(int port, bool incoming, int mapSize = 1024*1024*100, int maxDatabases = 5);
        Queue NonPersistentManagerFor(int port, bool incoming);
        Queue ManagerForReply();
        void Start(IEnumerable<LightningUri> uriList);

        void CreateQueue(LightningUri uri);
    }
}
