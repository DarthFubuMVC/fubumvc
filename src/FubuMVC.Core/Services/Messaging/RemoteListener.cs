using System;

namespace FubuMVC.Core.Services.Messaging
{
    public class RemoteListener : MarshalByRefObject, IRemoteListener
    {
        private readonly IMessagingHub _messagingHub;

        public RemoteListener(IMessagingHub messagingHub)
        {
            _messagingHub = messagingHub;
        }

        public void Send(string json)
        {
            try
            {
                _messagingHub.SendJson(json);
            }
            catch (Exception e)
            {
                Console.WriteLine("Remote messaging failed:");
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Really only for testing
        /// </summary>
        /// <param name="message"></param>
        public void SendObject(object message)
        {
            try
            {
                var json = JsonSerialization.ToJson(message);
                Send(json);
            }
            catch (Exception e)
            {
                Console.WriteLine("Remote json sending failed:");
                Console.WriteLine(e);
            }
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public T WaitForMessage<T>(Func<T, bool> filter, Action action, int wait = 5000)
        {
            return _messagingHub.WaitForMessage(filter, action, wait);
        }

    }
}