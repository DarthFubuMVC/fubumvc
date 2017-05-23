using System;
using System.Threading.Tasks;
using LightningQueues.Storage.LMDB;
using Shouldly;
using Xunit;

namespace LightningQueues.Tests.Storage.Lmdb
{
    [Collection("SharedTestDirectory")]
    public class OutgoingMessageScenarios : IDisposable
    {
        private readonly string _queuePath;
        private readonly LmdbMessageStore _store;

        public OutgoingMessageScenarios(SharedTestDirectory testDirectory)
        {
            _queuePath = testDirectory.CreateNewDirectoryForTest();
            _store = new LmdbMessageStore(_queuePath);
        }

        [Fact]
        public async Task happy_path_messages_sent()
        {
            var destination = new Uri("lq.tcp://localhost:5050");
            var message = ObjectMother.NewMessage<OutgoingMessage>("test");
            message.Destination = destination;
            var message2 = ObjectMother.NewMessage<OutgoingMessage>("test");
            message2.Destination = destination;
            message2.DeliverBy = DateTime.Now.AddSeconds(5);
            message2.MaxAttempts = 3;
            message2.Headers.Add("header", "headervalue");
            var tx = _store.BeginTransaction();
            _store.StoreOutgoing(tx, message);
            _store.StoreOutgoing(tx, message2);
            tx.Commit();
            _store.SuccessfullySent(message);

            var result = await _store.PersistedOutgoingMessages().FirstAsyncWithTimeout();
            result.Id.ShouldBe(message2.Id);
            result.Queue.ShouldBe(message2.Queue);
            result.Data.ShouldBe(message2.Data);
            result.SentAt.ShouldBe(message2.SentAt);
            result.DeliverBy.ShouldBe(message2.DeliverBy);
            result.MaxAttempts.ShouldBe(message2.MaxAttempts);
            result.Headers["header"].ShouldBe("headervalue");
        }

        [Fact]
        public void failed_to_send_with_max_attempts()
        {
            var destination = new Uri("lq.tcp://localhost:5050");
            var message = ObjectMother.NewMessage<OutgoingMessage>("test");
            message.MaxAttempts = 1;
            message.SentAttempts = 1;
            message.Destination = destination;
            var tx = _store.BeginTransaction();
            _store.StoreOutgoing(tx, message);
            tx.Commit();

            _store.FailedToSend(message);

            using (var ltx = _store.Environment.BeginTransaction())
            using(var db = ltx.OpenDatabase("outgoing"))
            {
                var result = ltx.Get(db, message.Id.MessageIdentifier.ToByteArray());
                result.ShouldBeNull();
            }
        }

        [Fact]
        public void failed_to_send_with_deliver_by()
        {
            var destination = new Uri("lq.tcp://localhost:5050");
            var message = ObjectMother.NewMessage<OutgoingMessage>("test");
            message.DeliverBy = DateTime.Now;
            message.Destination = destination;
            var tx = _store.BeginTransaction();
            _store.StoreOutgoing(tx, message);
            tx.Commit();

            _store.FailedToSend(message);

            using (var ltx = _store.Environment.BeginTransaction())
            using(var db = ltx.OpenDatabase("outgoing"))
            {
                var result = ltx.Get(db, message.Id.MessageIdentifier.ToByteArray());
                result.ShouldBeNull();
            }
        }

        public void Dispose()
        {
            _store.Dispose();
        }
    }
}