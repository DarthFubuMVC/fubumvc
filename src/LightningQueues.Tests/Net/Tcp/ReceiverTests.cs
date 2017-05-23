using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using LightningQueues.Net;
using LightningQueues.Net.Protocol.V1;
using LightningQueues.Net.Tcp;
using LightningQueues.Storage;
using LightningQueues.Storage.LMDB;
using Shouldly;
using Xunit;

namespace LightningQueues.Tests.Net.Tcp
{
    [Collection("SharedTestDirectory")]
    public class ReceiverTests : IDisposable
    {
        readonly IMessageStore _store;
        readonly IMessageStore _sendingStore;
        readonly SendingProtocol _sender;
        readonly Receiver _receiver;
        readonly IPEndPoint _endpoint;
        readonly RecordingLogger _logger;

        public ReceiverTests(SharedTestDirectory testDirectory)
        {
            var port = PortFinder.FindPort(); //to make it possible to run in parallel
            _endpoint = new IPEndPoint(IPAddress.Loopback, port);
            _logger = new RecordingLogger();
            _store = new LmdbMessageStore(testDirectory.CreateNewDirectoryForTest());
            _store.CreateQueue("test");
            _sendingStore = new LmdbMessageStore(testDirectory.CreateNewDirectoryForTest());
            _sendingStore.CreateQueue("test");
            _sender = new SendingProtocol(_sendingStore, _logger);
            var protocol = new ReceivingProtocol(_store, _logger);
            _receiver = new Receiver(_endpoint, protocol, _logger);
        }

        [Fact]
        public void stops_listening_on_dispose_of_subscription()
        {
            using(_receiver)
            using (_receiver.StartReceiving().Subscribe(x => { }))
            {
                try
                {
                    var listenerShouldThrow = new TcpListener(_endpoint);
                    listenerShouldThrow.Start();
                    true.ShouldBeFalse();
                }
                catch (Exception)
                {
                }
            }
            var listener = new TcpListener(_endpoint);
            listener.Start();
            listener.Stop();
        }

        [Fact]
        public void multiple_subscriptions_are_allowed()
        {
            using (_receiver.StartReceiving().Subscribe(x => { }))
            using (_receiver.StartReceiving().Subscribe(x => { }))
            {
            }
        }

        [Fact]
        public void subscribe_unsubscribe_and_subscribe_again()
        {
            using (_receiver.StartReceiving().Subscribe(x => { }))
            {
            }
            using (_receiver.StartReceiving().Subscribe(x => { }))
            {
            }
        }

        [Fact]
        public async Task can_handle_connect_then_disconnect()
        {
            using (_receiver.StartReceiving().Subscribe(x => true.ShouldBeFalse()))
            using (var client = new TcpClient())
            {
                await client.ConnectAsync(_endpoint.Address, _endpoint.Port);
            }
        }

        [Fact]
        public async Task can_handle_sending_three_bytes_then_disconnect()
        {
            using (_receiver.StartReceiving().Subscribe(x => true.ShouldBeFalse()))
            using (var client = new TcpClient())
            {
                await client.ConnectAsync(_endpoint.Address, _endpoint.Port);
                client.GetStream().Write(new byte[] { 1, 4, 6 }, 0, 3);
            }
        }

        [Fact]
        public async Task accepts_concurrently_connected_clients()
        {
            using (_receiver.StartReceiving().Subscribe(x => true.ShouldBeFalse()))
            using(var client1 = new TcpClient())
            using(var client2 = new TcpClient())
            {
                await client1.ConnectAsync(_endpoint.Address, _endpoint.Port);
                await client2.ConnectAsync(_endpoint.Address, _endpoint.Port);
                client2.GetStream().Write(new byte[] { 1, 4, 6 }, 0, 3);
                client1.GetStream().Write(new byte[] { 1, 4, 6 }, 0, 3);
            }
        }

        [Fact]
        public async Task receiving_a_valid_message()
        {
            var expected = ObjectMother.NewMessage<OutgoingMessage>("test");
            expected.Data = Encoding.UTF8.GetBytes("hello");
            expected.Destination = new Uri($"lq.tcp://localhost:{_endpoint.Port}");
            var tx = _store.BeginTransaction();
            _sendingStore.StoreOutgoing(tx, expected);
            tx.Commit();
            var messages = new[] {expected};
            var receivingCompletionSource = new TaskCompletionSource<Message>();
            using (_receiver.StartReceiving().Subscribe(x => { receivingCompletionSource.SetResult(x); }))
            using (var client = new TcpClient())
            {
                await client.ConnectAsync(_endpoint.Address, _endpoint.Port);
                var outgoing = new OutgoingMessageBatch(expected.Destination, messages, client);
                var completionSource = new TaskCompletionSource<bool>();

                using (_sender.Send(outgoing).Subscribe(x =>
                {
                    completionSource.SetResult(true);
                }))
                {
                    await Task.WhenAny(completionSource.Task, Task.Delay(100));
                }
                await Task.WhenAny(receivingCompletionSource.Task, Task.Delay(100));
            }
            receivingCompletionSource.Task.IsCompleted.ShouldBeTrue();
            var actual = receivingCompletionSource.Task.Result;
            actual.ShouldNotBeNull();
            actual.Id.ShouldBe(expected.Id);
            actual.Queue.ShouldBe(expected.Queue);
            Encoding.UTF8.GetString(actual.Data).ShouldBe("hello");
        }

        public void Dispose()
        {
            _receiver.Dispose();
            _store.Dispose();
            _sendingStore.Dispose();
        }
    }
}
