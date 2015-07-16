using System;
using System.Linq;
using FubuCore;

namespace FubuMVC.Core.Services.Messaging.Tracking
{
    public class MessageTrack
    {
        public static readonly string Sent = "Sent";
        public static readonly string Received = "Received";



        public static MessageTrack ForSent(object message, string id = null)
        {
            var track = derive(message, id);
            track.Status = Sent;
            return track;
        }

        private static MessageTrack derive(object message, string id)
        {
            var messageType = message.GetType();
            var track = new MessageTrack
            {
                Description = message.ToString(),
                FullName = messageType.FullName,
                Type = messageType.Name,
                Timestamp = DateTime.UtcNow,
                Id = id
            };

            if (id.IsEmpty())
            {
                autodetermineId(message, messageType, track);
            }

            return track;
        }

        private static void autodetermineId(object message, Type messageType, MessageTrack track)
        {
            var property = messageType.GetProperties().FirstOrDefault(x => FubuCore.StringExtensions.EqualsIgnoreCase(x.Name, "Id"));
            if (property != null)
            {
                var rawValue = property.GetValue(message, null);
                if (rawValue != null)
                {
                    track.Id = rawValue.ToString();
                }
            }
        }

        public static MessageTrack ForReceived(object message, string id = null)
        {
            var track = derive(message, id);
            track.Status = Received;
            return track;
        }

        public string FullName { get; set; }
        public string Type { get; set; }
        public DateTime Timestamp { get; set; }
        public string Id { get; set; }
        public string Description { get; set; }

        public string Status { get; set; }

        protected bool Equals(MessageTrack other)
        {
            return string.Equals(FullName, other.FullName) && string.Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MessageTrack) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((FullName != null ? FullName.GetHashCode() : 0)*397) ^ (Id != null ? Id.GetHashCode() : 0);
            }
        }
    }
}