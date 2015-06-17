using System.Threading;
using FubuTransportation.Polling;

namespace ScheduledJobHarness
{
    public class AlwaysGoodJob : IJob
    {
        public void Execute(CancellationToken cancellation)
        {
        }
    }
}