using System.IO;
using Newtonsoft.Json;

namespace FubuMVC.Core.ServiceBus.Runtime.Serializers
{
    public class JsonMessageSerializer : IMessageSerializer
    {
        private readonly JsonSerializer _serializer;

        public JsonMessageSerializer(TransportSettings settings)
        {
            _serializer = JsonSerializer.Create(settings.JsonMessageSerializerSettings);
        }

        public void Serialize(object message, Stream stream)
        {
            var writer = new StreamWriter(stream);
            _serializer.Serialize(writer, message);
            writer.Flush();
        }

        public object Deserialize(Stream message)
        {
            var reader = new JsonTextReader(new StreamReader(message));
            return _serializer.Deserialize(reader);
        }

        public string ContentType
        {
            get { return "application/json"; }
        }
    }
}
