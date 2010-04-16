using System;
using FubuMVC.Core.Behaviors;

namespace FubuMVC.Core.Registration.Nodes
{
    public class DeserializeJsonNode : Wrapper
    {
        private readonly Type _messageType;

        public DeserializeJsonNode(Type messageType)
            : base(typeof (DeserializeJsonBehavior<>).MakeGenericType(messageType))
        {
            _messageType = messageType;
        }

        public Type MessageType { get { return _messageType; } }

        public bool Equals(DeserializeJsonNode other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._messageType, _messageType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (DeserializeJsonNode)) return false;
            return Equals((DeserializeJsonNode) obj);
        }

        public override int GetHashCode()
        {
            return (_messageType != null ? _messageType.GetHashCode() : 0);
        }
    }
}