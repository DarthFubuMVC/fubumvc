using System;
using FubuCore;

namespace FubuMVC.Core.ServiceBus.Subscriptions
{
    public class Subscription
    {
        public static Subscription For<T>()
        {
            return new Subscription(typeof (T));
        }

        public Subscription(Type messageType)
        {
            MessageType = messageType.GetFullName();
        }

        public Subscription()
        {
        }

        public Guid Id { get; set; }
        public Uri Source { get; set; }
        public Uri Receiver { get; set; }
        public string MessageType { get; set; }
        public string NodeName { get; set; }
        public SubscriptionRole Role { get; set; }


        public Subscription Clone()
        {
           var clone = (Subscription) this.MemberwiseClone();
            clone.Id = Guid.Empty;

            return clone;
        }

        public Subscription SourcedFrom(Uri uri)
        {
            Source = uri;
            return this;
        }

        public Subscription ReceivedBy(Uri uri)
        {
            Receiver = uri;
            return this;
        }

        protected bool Equals(Subscription other)
        {
            return Equals(Source, other.Source) && Equals(Receiver, other.Receiver) && string.Equals(MessageType, other.MessageType) && string.Equals(NodeName, other.NodeName) && string.Equals(Role, other.Role);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Subscription) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Source != null ? Source.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Receiver != null ? Receiver.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (MessageType != null ? MessageType.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (NodeName != null ? NodeName.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return string.Format("Source: {0}, Receiver: {1}, MessageType: {2}, NodeName: {3}", Source, Receiver, MessageType, NodeName);
        }

        public bool Matches(Type inputType)
        {
            return inputType.GetFullName() == MessageType;
        }
    }
}