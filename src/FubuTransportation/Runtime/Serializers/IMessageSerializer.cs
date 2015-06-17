using System.IO;

namespace FubuTransportation.Runtime.Serializers
{
    public interface IMessageSerializer
    {
        void Serialize(object message, Stream stream);
        object Deserialize(Stream message);

        // TODO -- might need to change this to multiple mimetypes just like Conneg
        string ContentType { get; }
    }
}