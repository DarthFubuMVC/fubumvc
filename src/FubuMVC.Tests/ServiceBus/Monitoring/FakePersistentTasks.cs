using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FubuCore.Util;
using FubuMVC.Core.ServiceBus.Monitoring;

namespace FubuMVC.Tests.ServiceBus.Monitoring
{
    public class FakePersistentTasks : IPersistentTasks
    {
        public readonly Cache<Uri, FakePersistentTaskAgent> Agents =
            new Cache<Uri, FakePersistentTaskAgent>(_ => new FakePersistentTaskAgent(_));

        public IPersistentTask FindTask(Uri subject)
        {
            throw new NotImplementedException();
        }

        public IPersistentTaskAgent FindAgent(Uri subject)
        {
            return Agents[subject];
        }


        public IEnumerable<Uri> PersistentSubjects { get; set; }

        public string NodeId
        {
            get
            {
                return "FakeNode";
            }
        }

        public Task Reassign(Uri subject, IEnumerable<ITransportPeer> availablePeers, IEnumerable<ITransportPeer> deactivations)
        {
            throw new NotImplementedException();
        }

        public Task Reassign(Uri subject, IEnumerable<ITransportPeer> availablePeers)
        {
            throw new NotImplementedException();
        }
    }
}