using System.Linq;
using FubuCore.Logging;
using LightningQueues.Storage;
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
            using (var queues = new PersistentQueues(new RecordingLogger()))
            {
                queues.ClearAll();
                queues.Start(new LightningUri[]
                {
                    new LightningUri("lq.tcp://localhost:2424/some_queue"),
                    new LightningUri("lq.tcp://localhost:2424/other_queue"),
                    new LightningUri("lq.tcp://localhost:2424/third_queue"),
                });

                queues.PersistentManagerFor(2424, true)
                    .Queues.OrderBy(x => x).ShouldHaveTheSameElementsAs(LightningQueuesTransport.ErrorQueueName, "other_queue", "some_queue", "third_queue");
            }
        }

        [Test]
        public void creates_non_persistent_queues_when_started()
        {
            using (var queues = new PersistentQueues(new RecordingLogger()))
            {
                var queue = queues.NonPersistentManagerFor(2200, true);
                queues.Start(new []
                {
                    new LightningUri("lq.tcp://localhost:2200/some_queue"),
                    new LightningUri("lq.tcp://localhost:2200/other_queue"),
                    new LightningUri("lq.tcp://localhost:2200/third_queue"),
                });

                queue.Queues.OrderBy(x => x)
                    .ShouldHaveTheSameElementsAs(LightningQueuesTransport.ErrorQueueName, "other_queue", "some_queue",
                        "third_queue");
                queue.Store.ShouldBeOfType<NoStorage>();
            }
        }
    }
}