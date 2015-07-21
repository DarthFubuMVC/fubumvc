using System.IO;

namespace FubuMVC.Core.ServiceBus.Runtime.Serializers
{
    public interface IMessageSerializer
    {
        void Serialize(object message, Stream stream);
        object Deserialize(Stream message);

        // TODO -- might need to change this to multiple mimetypes just like Conneg
        string ContentType { get; }
    }
}