using System;
using System.Collections.Specialized;
using System.IO;
using FubuCore;
using FubuMVC.Core.ServiceBus.Runtime;
using FubuMVC.Core.ServiceBus.Runtime.Serializers;

namespace FubuMVC.Core.ServiceBus.ErrorHandling
{
    [Serializable]
    public class ErrorReport
    {
        public const string ExceptionDetected = "Exception Detected";

        public ErrorReport(Envelope envelope, Exception ex)
        {
            Headers = envelope.Headers.ToNameValues();
            ExceptionText = ex.ToString();
            ExceptionMessage = ex.Message;
            ExceptionType = ex.GetType().FullName;
            Explanation = ExceptionDetected;
            RawData = envelope.Data;
            Message = envelope.Message;
        }

        [NonSerialized] public object Message; // leave it like this please

        public byte[] RawData { get; set; }

        public NameValueCollection Headers { get; set; }

        public string Explanation { get; set; }

        public string ExceptionType { get; set; }
        public string ExceptionMessage { get; set; }
        public string ExceptionText { get; set; }

        protected bool Equals(ErrorReport other)
        {
            return Equals(Message, other.Message) && string.Equals(ExceptionText, other.ExceptionText);
        }

        public byte[] Serialize()
        {
            using (var stream = new MemoryStream())
            {
                new BinarySerializer().Serialize(this, stream);
                stream.Position = 0;
                return stream.ReadAllBytes();
            }

            
        }

        public static ErrorReport Deserialize(byte[] data)
        {
            var stream = new MemoryStream(data);

            return (ErrorReport) new BinarySerializer().Deserialize(stream);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ErrorReport) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Message != null ? Message.GetHashCode() : 0)*397) ^ (ExceptionText != null ? ExceptionText.GetHashCode() : 0);
            }
        }
    }
}