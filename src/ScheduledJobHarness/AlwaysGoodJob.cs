using System.Threading;
using FubuMVC.Core.ServiceBus.Polling;

namespace ScheduledJobHarness
{
    public class AlwaysGoodJob : IJob
    {
        public void Execute(CancellationToken cancellation)
        {
        }
    }
}