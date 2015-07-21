using System;
using FubuCore.Descriptions;
using FubuCore.Logging;

namespace FubuMVC.Core.ServiceBus.Logging
{
    public class ReceiveFailed : LogRecord, DescribesItself
    {
        /// <summary>
        ///     The Key of the ChannelNode that had the receive failure.
        /// </summary>
        public string ChannelKey { get; set; }

        public Exception Exception { get; set; }


        protected bool Equals(ReceiveFailed other)
        {
            return string.Equals(ChannelKey, other.ChannelKey) && Equals(Exception, other.Exception);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ReceiveFailed)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((ChannelKey != null ? ChannelKey.GetHashCode() : 0) * 397)
                       ^ (Exception != null ? Exception.GetHashCode() : 0);
            }
        }

        public void Describe(Description description)
        {
            description.Title = ToString();
            description.ShortDescription = Exception.Message;
            description.LongDescription = Exception.ToString();
        }

        public override string ToString()
        {
            return "Receive failed on Channel: " + ChannelKey;
        }
    }
}