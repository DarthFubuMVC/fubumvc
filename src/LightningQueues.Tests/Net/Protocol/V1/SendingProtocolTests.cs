using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using LightningQueues.Logging;
using LightningQueues.Net.Protocol.V1;
using LightningQueues.Storage;
using LightningQueues.Storage.LMDB;
using Shouldly;
using Xunit;

namespace LightningQueues.Tests.Net.Protocol.V1
{
    [Collection("SharedTestDirectory")]
    public class SendingProtocolTests : IDisposable
    {
        readonly SendingProtocol _sender;
        readonly IMessageStore _store;

        public SendingProtocolTests(SharedTestDirectory testDirectory)
        {
            _store = new LmdbMessageStore(testDirectory.CreateNewDirectoryForTest());
            _sender = new SendingProtocol(_store, new NulloLogger());
        }

        [Fact]
        public async Task writing_the_length()
        {
            var expected = 5;
            using (var ms = new MemoryStream())
            {
                await _sender.WriteLength(ms, 5).FirstAsyncWithTimeout();
                var actual = BitConverter.ToInt32(ms.ToArray(), 0);
                actual.ShouldBe(expected);
            }
        }

        [Fact]
        public async Task writing_the_message_bytes()
        {
            var expected = new byte[] { 1, 4, 6 };
            using (var ms = new MemoryStream())
            {
                await _sender.WriteMessages(ms, expected).FirstAsyncWithTimeout();
                var actual = ms.ToArray();
                actual.SequenceEqual(expected).ShouldBeTrue();
            }
        }

        [Fact]
        public async Task receive_happy()
        {
            using (var ms = new MemoryStream())
            {
                ms.Write(Constants.ReceivedBuffer, 0, Constants.ReceivedBuffer.Length);
                ms.Position = 0;
                var result = await _sender.ReadReceived(ms).FirstAsyncWithTimeout();
                result.ShouldBe(Unit.Default);
            }
        }

        [Fact]
        public async Task receive_not_happy()
        {
            using (var ms = new MemoryStream())
            {
                await Assert.ThrowsAsync<TimeoutException>(() => _sender.ReadReceived(ms).FirstAsyncWithTimeout(TimeSpan.FromMilliseconds(5)));
            }
        }

        [Fact]
        public async Task sending_acknowledgement()
        {
            using (var ms = new MemoryStream())
            {
                await _sender.WriteAcknowledgement(ms).FirstAsyncWithTimeout();
                ms.ToArray().SequenceEqual(Constants.AcknowledgedBuffer).ShouldBeTrue();
            }
        }

        public void Dispose()
        {
            _store.Dispose();
        }
    }
}