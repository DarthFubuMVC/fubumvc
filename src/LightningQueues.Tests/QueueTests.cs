using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using LightningQueues.Storage.LMDB;
using Microsoft.Reactive.Testing;
using Shouldly;
using Xunit;

namespace LightningQueues.Tests
{
    [Collection("SharedTestDirectory")]
    public class QueueTests : IDisposable
    {
        private readonly SharedTestDirectory _testDirectory;
        private readonly TestScheduler _scheduler;
        private readonly Queue _queue;

        public QueueTests(SharedTestDirectory testDirectory)
        {
            _testDirectory = testDirectory;
            _scheduler = new TestScheduler();
            _queue = ObjectMother.NewQueue(testDirectory.CreateNewDirectoryForTest(), scheduler: _scheduler);
        }

        [Fact]
        public void receive_at_a_later_time()
        {
            var received = false;
            _queue.ReceiveLater(new Message {Queue = "test", Id = MessageId.GenerateRandom()}, TimeSpan.FromSeconds(3));
            using (_queue.Receive("test").Subscribe(x => received = true))
            {
                _scheduler.AdvanceBy(TimeSpan.FromSeconds(2).Ticks);
                received.ShouldBeFalse();
                _scheduler.AdvanceBy(TimeSpan.FromSeconds(1).Ticks);
                received.ShouldBeTrue();
            }
        }

        [Fact]
        public void receive_at_a_specified_time()
        {
            var received = false;
            var time = DateTimeOffset.Now.AddSeconds(5);
            _queue.ReceiveLater(new Message {Queue = "test", Id = MessageId.GenerateRandom()}, time);
            using (_queue.Receive("test").Subscribe(x => received = true))
            {
                _scheduler.AdvanceBy(time.AddSeconds(-3).Ticks);
                received.ShouldBeFalse();
                _scheduler.AdvanceBy(time.AddSeconds(-2).Ticks);
                received.ShouldBeTrue();
            }
        }

        [Fact]
        public void enqueue_a_message()
        {
            Message result = null;
            var expected = ObjectMother.NewMessage<Message>("test");
            using (_queue.Receive("test").Subscribe(x => result = x.Message))
            {
                _queue.Enqueue(expected);
            }
            expected.ShouldBeSameAs(result);
        }

        [Fact]
        public void moving_queues()
        {
            _queue.CreateQueue("another");
            Message first = null;
            Message afterMove = null;
            var expected = ObjectMother.NewMessage<Message>("test");
            using (_queue.Receive("another").Subscribe(x => afterMove = x.Message))
            using (_queue.Receive("test").Subscribe(x => first = x.Message))
            {
                _queue.Enqueue(expected);
                _queue.MoveToQueue("another", first);
            }
            afterMove.Queue.ShouldBe("another");
        }

        [Fact]
        public async Task send_message_to_self()
        {
            using (var queue = ObjectMother.NewQueue(_testDirectory.CreateNewDirectoryForTest()))
            {
                var message = ObjectMother.NewMessage<OutgoingMessage>("test");
                message.Destination = new Uri($"lq.tcp://localhost:{queue.Endpoint.Port}");
                queue.Send(message);
                var received = await queue.Receive("test").FirstAsyncWithTimeout();
                received.ShouldNotBeNull();
                received.Message.Queue.ShouldBe(message.Queue);
                received.Message.Data.ShouldBe(message.Data);
            }
        }

        [Fact]
        public async Task sending_to_bad_endpoint_no_retries_integration_test()
        {
            using (var queue = ObjectMother.NewQueue(_testDirectory.CreateNewDirectoryForTest()))
            {
                var message = ObjectMother.NewMessage<OutgoingMessage>("test");
                message.MaxAttempts = 1;
                message.Destination = new Uri($"lq.tcp://boom:{queue.Endpoint.Port + 1}");
                queue.Send(message);
                await Task.Delay(TimeSpan.FromSeconds(10));
                var store = (LmdbMessageStore) queue.Store;
                store.PersistedOutgoingMessages().ToEnumerable().Any().ShouldBeFalse();
            }
        }

        [Fact(Skip = "Doesn't work on current travis ci OS")]
        public void can_start_two_instances_for_IIS_stop_and_start()
        {
            //This shows that the port doesn't have an exclusive lock, and that lmdb itself can have multiple instances
            var path = _testDirectory.CreateNewDirectoryForTest();
            var store = new LmdbMessageStore(path);
            var queueConfiguration = new QueueConfiguration();
            queueConfiguration.LogWith(new RecordingLogger());
            queueConfiguration.AutomaticEndpoint();
            queueConfiguration.StoreMessagesWith(store);
            var queue = queueConfiguration.BuildQueue();
            var queue2 = queueConfiguration.BuildQueue();
            using(queue)
            using (queue2)
            {
                queue.CreateQueue("test");
                queue.Start();
                queue2.CreateQueue("test");
                queue2.Start();
                using(queue.Receive("test").Subscribe(x => { }))
                using (queue2.Receive("test").Subscribe(x => { }))
                {
                    
                }
            }
        }

        public void Dispose()
        {
            _queue.Dispose();
        }
    }
}