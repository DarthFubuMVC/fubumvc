using System;
using System.Linq;
using System.Reactive.Linq;
using LightningQueues.Storage.LMDB;
using Shouldly;
using Xunit;

namespace LightningQueues.Tests.Storage.Lmdb
{
    [Collection("SharedTestDirectory")]
    public class LmdbMessageStoreTester : IDisposable
    {
        private readonly LmdbMessageStore _store;
        private readonly string _path;

        public LmdbMessageStoreTester(SharedTestDirectory testDirectory)
        {
            _path = testDirectory.CreateNewDirectoryForTest();
            _store = new LmdbMessageStore(_path);
        }

        [Fact]
        public void getting_all_queues()
        {
            _store.CreateQueue("test");
            _store.CreateQueue("test2");
            _store.CreateQueue("test3");
            _store.GetAllQueues().SequenceEqual(new[] {"test", "test2", "test3"}).ShouldBeTrue();
        }

        [Fact]
        public void clear_all_history_with_empty_dataset_doesnt_throw()
        {
            _store.CreateQueue("test");
            _store.ClearAllStorage();
        }

        [Fact]
        public void clear_all_history_with_persistent_data()
        {
            _store.CreateQueue("test");
            var message = ObjectMother.NewMessage<Message>("test");
            var outgoingMessage = ObjectMother.NewMessage<OutgoingMessage>();
            outgoingMessage.Destination = new Uri("lq.tcp://localhost:3030");
            outgoingMessage.SentAt = DateTime.Now;
            var tx = _store.BeginTransaction();
            _store.StoreOutgoing(tx, outgoingMessage);
            _store.StoreIncomingMessages(tx, message);
            tx.Commit();
            _store.PersistedMessages("test").ToEnumerable().Count().ShouldBe(1);
            _store.PersistedOutgoingMessages().ToEnumerable().Count().ShouldBe(1);
            _store.ClearAllStorage();
            _store.PersistedMessages("test").ToEnumerable().Count().ShouldBe(0);
            _store.PersistedOutgoingMessages().ToEnumerable().Count().ShouldBe(0);
        }

        [Fact]
        public void store_can_read_previously_stored_items()
        {
            _store.CreateQueue("test");
            var message = ObjectMother.NewMessage<Message>("test");
            var outgoingMessage = ObjectMother.NewMessage<OutgoingMessage>();
            outgoingMessage.Destination = new Uri("lq.tcp://localhost:3030");
            outgoingMessage.SentAt = DateTime.Now;
            var tx = _store.BeginTransaction();
            _store.StoreOutgoing(tx, outgoingMessage);
            _store.StoreIncomingMessages(tx, message);
            tx.Commit();
            _store.Dispose();
            using (var store2 = new LmdbMessageStore(_path))
            {
                store2.PersistedMessages("test").ToEnumerable().Count().ShouldBe(1);
                store2.PersistedOutgoingMessages().ToEnumerable().Count().ShouldBe(1);
            }
        }

        [Fact]
        public void retrieve_message_by_id()
        {
            _store.CreateQueue("test");
            var message = ObjectMother.NewMessage<Message>("test");
            var outgoingMessage = ObjectMother.NewMessage<OutgoingMessage>();
            outgoingMessage.Destination = new Uri("lq.tcp://localhost:3030");
            outgoingMessage.SentAt = DateTime.Now;
            var tx = _store.BeginTransaction();
            _store.StoreOutgoing(tx, outgoingMessage);
            _store.StoreIncomingMessages(tx, message);
            tx.Commit();
            var message2 = _store.GetMessage(message.Queue, message.Id);
            var outgoing2 = _store.GetMessage("outgoing", outgoingMessage.Id);
            message2.ShouldNotBeNull();
            outgoing2.ShouldNotBeNull();
        }

        public void Dispose()
        {
            _store.Dispose();
        }
    }
}