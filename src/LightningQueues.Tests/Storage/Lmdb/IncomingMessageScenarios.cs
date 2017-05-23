using System;
using System.Linq;
using LightningQueues.Serialization;
using LightningQueues.Storage;
using LightningQueues.Storage.LMDB;
using Shouldly;
using Xunit;
using static LightningQueues.Tests.ObjectMother;

namespace LightningQueues.Tests.Storage.Lmdb
{
    [Collection("SharedTestDirectory")]
    public class IncomingMessageScenarios : IDisposable
    {
        private readonly string _queuePath;
        private LmdbMessageStore _store;

        public IncomingMessageScenarios(SharedTestDirectory testDirectory)
        {
            _queuePath = testDirectory.CreateNewDirectoryForTest();
            _store = new LmdbMessageStore(_queuePath);
        }

        [Fact]
        public void happy_path_success()
        {
            var message = NewMessage<Message>();
            _store.CreateQueue(message.Queue);
            var transaction = _store.BeginTransaction();
            _store.StoreIncomingMessages(transaction, message);
            transaction.Commit();
            using (var tx = _store.Environment.BeginTransaction())
            {
                using (var db = tx.OpenDatabase(message.Queue))
                {
                    var msg = tx.Get(db, message.Id.MessageIdentifier.ToByteArray()).ToMessage();
                    System.Text.Encoding.UTF8.GetString(msg.Data).ShouldBe("hello");
                    msg.Headers.First().Value.ShouldBe("myvalue");
                }
            }
        }

        [Fact]
        public void storing_message_for_queue_that_doesnt_exist()
        {
            var message = NewMessage<Message>();
            Assert.Throws<QueueDoesNotExistException>(() =>
            {
                var tx = _store.BeginTransaction();
                _store.StoreIncomingMessages(tx, message);
            });
        }

        [Fact]
        public void crash_before_commit()
        {
            var message = NewMessage<Message>();
            _store.CreateQueue(message.Queue);
            var transaction = _store.BeginTransaction();
            _store.StoreIncomingMessages(transaction, message);
            _store.Dispose();
            //crash
            _store = new LmdbMessageStore(_queuePath);
            using (var tx = _store.Environment.BeginTransaction())
            {
                using (var db = tx.OpenDatabase(message.Queue))
                {
                    var result = tx.Get(db, message.Id.MessageIdentifier.ToByteArray());
                    result.ShouldBeNull();
                }
            }
        }

        [Fact]
        public void rollback_messages_received()
        {
            var message = NewMessage<Message>();
            _store.CreateQueue(message.Queue);
            var transaction = _store.BeginTransaction();
            _store.StoreIncomingMessages(transaction, message);
            transaction.Rollback();
            using (var tx = _store.Environment.BeginTransaction())
            {
                using (var db = tx.OpenDatabase(message.Queue))
                {
                    var result = tx.Get(db, message.Id.MessageIdentifier.ToByteArray());
                    result.ShouldBeNull();
                }
            }
        }

        [Fact]
        public void creating_multiple_stores()
        {
            _store.Dispose();
            _store = new LmdbMessageStore(_queuePath);
            _store.Dispose();
            _store = new LmdbMessageStore(_queuePath);
        }

        public void Dispose()
        {
            _store.Dispose();
        }
    }
}