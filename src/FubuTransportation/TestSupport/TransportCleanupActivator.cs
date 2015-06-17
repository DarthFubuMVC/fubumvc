using System.Collections.Generic;
using Bottles;
using Bottles.Diagnostics;

namespace FubuTransportation.TestSupport
{
    public class TransportCleanupActivator : IActivator
    {
        private readonly TransportCleanup _cleanup;

        public TransportCleanupActivator(TransportCleanup cleanup)
        {
            _cleanup = cleanup;
        }

        public void Activate(IEnumerable<IPackageInfo> packages, IPackageLog log)
        {
            log.Trace("Adding TransportCleanup to the Bottles EventAggregator");
            Bottles.Services.Messaging.EventAggregator.Messaging.AddListener(_cleanup);
        }
    }
}