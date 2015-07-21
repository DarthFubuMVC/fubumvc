using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace FubuMVC.Core.ServiceBus.Runtime.Serializers
{
    public class BinarySerializer : IMessageSerializer
    {
        public void Serialize(object message, Stream stream)
        {
            var serializer = new BinaryFormatter();
            serializer.Serialize(stream, message);
        }

        public object Deserialize(Stream message)
        {
            return new BinaryFormatter().Deserialize(message);
        }

        public string ContentType
        {
            get { return "binary/octet-stream"; }
        }
    }
}