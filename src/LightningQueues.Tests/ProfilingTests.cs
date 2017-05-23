using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using System.Reactive.Linq;
using LightningQueues.Logging;
using Xunit;

namespace LightningQueues.Tests
{
    [Collection("SharedTestDirectory")]
    public class ProfilingTests : IDisposable
    {
        private readonly Queue _sender;
        private readonly Queue _receiver;

        public ProfilingTests(SharedTestDirectory testDirectory)
        {
            _sender = ObjectMother.NewQueue(testDirectory.CreateNewDirectoryForTest(), logger: new NulloLogger());
            _receiver = ObjectMother.NewQueue(testDirectory.CreateNewDirectoryForTest(), logger: new NulloLogger());
        }

        [Fact(Skip = "only when needed"), Trait("prof", "explicit")]
        public async Task messages_totaling()
        {
            var stopwatch = Stopwatch.StartNew();
            await messages_totaling_helper(1000);
            stopwatch.Stop();
            Console.WriteLine($"Total time is {stopwatch.Elapsed.TotalMilliseconds}");
            //Console.WriteLine("Get new snapshot for comparison and press enter when done.");
            //Console.ReadLine();
        }

        private async Task messages_totaling_helper(int numberOfMessages)
        {
            var taskCompletionSource = new TaskCompletionSource<bool>();
            var subscription = _receiver.Receive("test").ObserveOn(TaskPoolScheduler.Default).Do(x =>
            {
                x.QueueContext.SuccessfullyReceived();
                x.QueueContext.CommitChanges();
            }).RunningCount().Subscribe(x =>
            {
                if (x == numberOfMessages)
                    taskCompletionSource.SetResult(true);
            });

            //Console.WriteLine("Get a baseline snapshot for comparison and press enter when done.");
            //Console.ReadLine();
            var messages = new List<OutgoingMessage>();
            var destination = new Uri($"lq.tcp://localhost:{_receiver.Endpoint.Port}");
            for (var i = 0; i < numberOfMessages; ++i)
            {
                var message = ObjectMother.NewMessage<OutgoingMessage>("test");
                message.Destination = destination;
                messages.Add(message);
            }
            _sender.Send(messages.ToArray());
            await taskCompletionSource.Task;
            subscription.Dispose();
        }

        public void Dispose()
        {
            _sender.Dispose();
            _receiver.Dispose();
        }
    }
}