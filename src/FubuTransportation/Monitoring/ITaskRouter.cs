using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FubuTransportation.Monitoring
{
    public interface ITaskRouter
    {
        Task<ITransportPeer> SelectOwner(Uri subject, IEnumerable<ITransportPeer> peers);
    }
}