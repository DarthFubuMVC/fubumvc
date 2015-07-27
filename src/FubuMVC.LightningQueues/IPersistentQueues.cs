using System;
using System.Collections.Generic;
using FubuMVC.Core.ServiceBus.Runtime;
using LightningQueues;

namespace FubuMVC.LightningQueues
{
    public interface IPersistentQueues : IDisposable
    {
        IEnumerable<IQueueManager> AllQueueManagers { get; } 
        void ClearAll();
        IQueueManager ManagerFor(int port, bool incoming);
        IQueueManager ManagerForReply();
        void Start(IEnumerable<LightningUri> uriList);

        void CreateQueue(LightningUri uri);
        IEnumerable<EnvelopeToken> ReplayDelayed(DateTime currentTime);
    }
}