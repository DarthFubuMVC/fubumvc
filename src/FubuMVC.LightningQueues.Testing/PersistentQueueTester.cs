using System.Linq;
using FubuCore.Logging;
using NUnit.Framework;
using Shouldly;

namespace FubuMVC.LightningQueues.Testing
{
    [TestFixture]
    public class PersistentQueueTester
    {
        [Test]
        public void creates_queues_when_started()
        {
            using (var queues = new PersistentQueues())
            {
                queues.ClearAll();
                queues.Start(new LightningUri[]
                {
                    new LightningUri("lq.tcp://localhost:2424/some_queue"),
                    new LightningUri("lq.tcp://localhost:2424/other_queue"),
                    new LightningUri("lq.tcp://localhost:2424/third_queue"),
                });

                queues.ManagerFor(2424, true)
                    .Queues.OrderBy(x => x).ShouldHaveTheSameElementsAs(LightningQueuesTransport.ErrorQueueName, "other_queue", "some_queue", "third_queue");
            }
        }
    }
}