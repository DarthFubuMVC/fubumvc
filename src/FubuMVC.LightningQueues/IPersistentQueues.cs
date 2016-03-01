using System;
using System.Collections.Generic;
using LightningQueues;

namespace FubuMVC.LightningQueues
{
    public interface IPersistentQueues : IDisposable
    {
        IEnumerable<Queue> AllQueueManagers { get; }
        void ClearAll();
        Queue ManagerFor(int port, bool incoming);
        Queue ManagerForReply();
        void Start(IEnumerable<LightningUri> uriList);

        void CreateQueue(LightningUri uri);
    }
}