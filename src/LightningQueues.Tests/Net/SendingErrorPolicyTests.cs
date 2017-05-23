using System;
using System.Linq;
using System.Net.Sockets;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using LightningQueues.Logging;
using LightningQueues.Net;
using LightningQueues.Storage;
using LightningQueues.Storage.LMDB;
using Microsoft.Reactive.Testing;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Shouldly;
using Xunit;

namespace LightningQueues.Tests.Net
{
    [Collection("SharedTestDirectory")]
    public class SendingErrorPolicyTests : IDisposable
    {
        private readonly ILogger _logger;
        private readonly SendingErrorPolicy _errorPolicy;
        private readonly TestScheduler _scheduler;
        private readonly LmdbMessageStore _store;
        private readonly Subject<OutgoingMessageFailure> _subject;

        public SendingErrorPolicyTests(SharedTestDirectory testDirectory)
        {
            _logger = new RecordingLogger();
            _scheduler = new TestScheduler();
            _store = new LmdbMessageStore(testDirectory.CreateNewDirectoryForTest());
            _subject = new Subject<OutgoingMessageFailure>();
            _errorPolicy = new SendingErrorPolicy(_logger, _store, _subject, _scheduler);
        }

        [Fact]
        public void max_attempts_is_reached()
        {
            var message = ObjectMother.NewMessage<OutgoingMessage>();
            message.MaxAttempts = 3;
            message.SentAttempts = 3;
            _errorPolicy.ShouldRetry(message).ShouldBeFalse();
        }

        [Fact]
        public void max_attempts_is_not_reached()
        {
            var message = ObjectMother.NewMessage<OutgoingMessage>();
            message.MaxAttempts = 20;
            message.SentAttempts = 5;
            _errorPolicy.ShouldRetry(message).ShouldBeTrue();
        }

        [Fact]
        public void deliver_by_has_expired()
        {
            var message = ObjectMother.NewMessage<OutgoingMessage>();
            message.DeliverBy = DateTime.Now.Subtract(TimeSpan.FromSeconds(1));
            message.SentAttempts = 5;
            _errorPolicy.ShouldRetry(message).ShouldBeFalse();
        }

        [Fact]
        public void deliver_by_has_not_expired()
        {
            var message = ObjectMother.NewMessage<OutgoingMessage>();
            message.DeliverBy = DateTime.Now.Add(TimeSpan.FromSeconds(1));
            message.SentAttempts = 5;
            _errorPolicy.ShouldRetry(message).ShouldBeTrue();
        }

        [Fact]
        public void has_neither_deliverby_nor_max_attempts()
        {
            var message = ObjectMother.NewMessage<OutgoingMessage>();
            message.SentAttempts = 5;
            _errorPolicy.ShouldRetry(message).ShouldBeTrue();
        }

        [Fact]
        public void message_is_observed_after_time()
        {
            Message observed = null;
            var message = ObjectMother.NewMessage<OutgoingMessage>();
            message.Destination = new Uri("lq.tcp://localhost:5150/blah");
            message.MaxAttempts = 2;
            var tx = _store.BeginTransaction();
            _store.StoreOutgoing(tx, message);
            tx.Commit();
            var failure = new OutgoingMessageFailure();
            failure.Batch = new OutgoingMessageBatch(message.Destination, new []{message}, new TcpClient());
            using (_errorPolicy.RetryStream.Subscribe(x => { observed = x; }))
            {
                _subject.OnNext(failure);
                _scheduler.AdvanceBy(TimeSpan.FromSeconds(1).Ticks);
                observed.ShouldNotBeNull();
            }
        }

        [Fact]
        public void message_removed_from_storage_after_max()
        {
            Message observed = null;
            var message = ObjectMother.NewMessage<OutgoingMessage>();
            message.Destination = new Uri("lq.tcp://localhost:5150/blah");
            message.MaxAttempts = 1;
            var tx = _store.BeginTransaction();
            _store.StoreOutgoing(tx, message);
            tx.Commit();
            var failure = new OutgoingMessageFailure();
            failure.Batch = new OutgoingMessageBatch(message.Destination, new[] {message}, new TcpClient());
            using (_errorPolicy.RetryStream.Subscribe(x => { observed = x; }))
            {
                _subject.OnNext(failure);
                _scheduler.AdvanceBy(TimeSpan.FromSeconds(1).Ticks);
                observed.ShouldBeNull();
            }
            _store.PersistedOutgoingMessages().ToEnumerable().Any().ShouldBeFalse();
        }

        [Fact]
        public void time_increases_with_each_failure()
        {
            Message observed = null;
            var message = ObjectMother.NewMessage<OutgoingMessage>();
            message.Destination = new Uri("lq.tcp://localhost:5150/blah");
            message.MaxAttempts = 5;
            var tx = _store.BeginTransaction();
            _store.StoreOutgoing(tx, message);
            tx.Commit();
            var failure = new OutgoingMessageFailure();
            failure.Batch = new OutgoingMessageBatch(message.Destination, new[] {message}, new TcpClient());
            using (_errorPolicy.RetryStream.Subscribe(x => { observed = x; }))
            {
                _subject.OnNext(failure);
                _scheduler.AdvanceBy(TimeSpan.FromSeconds(1).Ticks);
                observed.ShouldNotBeNull("first");
                observed = null;
                _subject.OnNext(failure);
                observed.ShouldBeNull("second");
                _scheduler.AdvanceBy(TimeSpan.FromSeconds(1).Ticks); //one second isn't enough yet
                observed.ShouldBeNull("third");
                _scheduler.AdvanceBy(TimeSpan.FromSeconds(3).Ticks); //four seconds total for second failure should match
                observed.ShouldNotBeNull("fourth");
            }
        }

        [Fact]
        public void errors_in_storage_dont_end_stream()
        {
            var message = ObjectMother.NewMessage<OutgoingMessage>();
            var store = Substitute.For<IMessageStore>();
            store.FailedToSend(Arg.Is(message)).Throws(new Exception("bam!"));
            var errorPolicy = new SendingErrorPolicy(new RecordingLogger(), store, _subject, _scheduler);
            bool ended = false;
            var failure = new OutgoingMessageFailure();
            failure.Batch = new OutgoingMessageBatch(message.Destination, new[] {message}, new TcpClient());
            using (errorPolicy.RetryStream.Finally(() => ended = true).Subscribe(x => { }))
            {
                _subject.OnNext(failure);
                _scheduler.AdvanceBy(TimeSpan.FromSeconds(1).Ticks);
                ended.ShouldBeFalse();
            }
        }

        public void Dispose()
        {
            _store.Dispose();
        }
    }
}