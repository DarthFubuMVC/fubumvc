using System;
using FubuMVC.Core.Registration.Nodes;

namespace FubuMVC.Core.Runtime.Aggregation
{
    public class ClientMessagePath
    {
        public string Message { get; set; }
        public Type InputType { get; set; }
        public Type ResourceType { get; set; }
        public BehaviorChain Chain { get; set; }

        protected bool Equals(ClientMessagePath other)
        {
            return string.Equals(Message, other.Message) && Equals(InputType, other.InputType) && Equals(ResourceType, other.ResourceType);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ClientMessagePath) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Message != null ? Message.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (InputType != null ? InputType.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (ResourceType != null ? ResourceType.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format("Message: {0}, InputType: {1}, ResourceType: {2}", Message, InputType, ResourceType);
        }
    }
}