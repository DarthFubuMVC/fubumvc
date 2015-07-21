using System;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.ServiceBus.Web
{
    public class SendsMessage : Process
    {
        private readonly Type _eventType;

        public SendsMessage(ActionCall transform) : base(typeof(SendMessageBehavior<>).MakeGenericType(transform.ResourceType()))
        {
            _eventType = transform.ResourceType();
        }

        public Type EventType
        {
            get { return _eventType; }
        }
    }
}