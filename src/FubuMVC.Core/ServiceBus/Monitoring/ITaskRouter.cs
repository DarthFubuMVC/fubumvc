using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FubuMVC.Core.ServiceBus.Monitoring
{
    public interface ITaskRouter
    {
        Task<ITransportPeer> SelectOwner(Uri subject, IEnumerable<ITransportPeer> peers);
    }
}