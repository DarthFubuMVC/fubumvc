using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FubuTransportation.Monitoring
{
    public interface IPersistentTask
    {
        Uri Subject { get; }
        void AssertAvailable();
        void Activate();
        void Deactivate();
        bool IsActive { get; }

        Task<ITransportPeer> SelectOwner(IEnumerable<ITransportPeer> peers);
    }
}