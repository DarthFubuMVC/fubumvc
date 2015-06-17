using System.Threading;
using FubuCore.Dates;
using FubuTransportation.Polling;

namespace FubuTransportation.Events
{
    public class ExpiringListenerCleanup : IJob
    {
        private readonly IEventAggregator _events;
        private readonly ISystemTime _systemTime;

        public ExpiringListenerCleanup(IEventAggregator events, ISystemTime systemTime)
        {
            _events = events;
            _systemTime = systemTime;
        }

        public void Execute(CancellationToken cancellation)
        {
            _events.PruneExpiredListeners(_systemTime.UtcNow());
        }
    }
}