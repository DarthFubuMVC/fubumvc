using System;
using LightningQueues.Model;
using LightningQueues.Protocol;

namespace LightningQueues
{
    public class MessageEventArgs : EventArgs
    {
        public MessageEventArgs(Endpoint endpoint, Message message)
        {
            Endpoint = endpoint;
            Message = message;
        }

        public Endpoint Endpoint { get; private set; }
        public Message Message { get; private set; }
    }
}