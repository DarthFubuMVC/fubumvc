using System.Text;
using System.Xml;
using System.Xml.Serialization;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Conneg
{
    // See the StoryTeller tests for conneg
    public class XmlFormatter : IFormatter
    {
        private readonly IStreamingData _streaming;

        public XmlFormatter(IStreamingData streaming)
        {
            _streaming = streaming;
        }

        public T Read<T>(CurrentRequest currentRequest)
        {
            var serializer = new XmlSerializer(typeof (T));
            return (T) serializer.Deserialize(_streaming.Input);
        }

        public void Write<T>(T target, CurrentRequest request)
        {
            var serializer = new XmlSerializer(typeof(T));

            // TODO -- later, we'll need to get more sophisticated and worry about the Encoding
            var xmlWriter = new XmlTextWriter(_streaming.Output, Encoding.Default){
                Formatting = Formatting.None
            };

            serializer.Serialize(xmlWriter, target);

            _streaming.OutputContentType = request.ContentType;
        }

        public bool Matches(CurrentRequest request)
        {
            return request.MatchesOneOfTheseMimeTypes("text/xml", "application/xml");
        }
    }
}