using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Resources.Media.Formatters
{
    // See the StoryTeller tests for conneg
    public class XmlFormatter : IFormatter
    {
        private readonly IStreamingData _streaming;

        public XmlFormatter(IStreamingData streaming)
        {
            _streaming = streaming;
        }

        public void Write<T>(T target, string mimeType)
        {
            var serializer = new XmlSerializer(typeof (T));

            // TODO -- later, we'll need to get more sophisticated and worry about the Encoding
            var xmlWriter = new XmlTextWriter(_streaming.Output, Encoding.Default){
                Formatting = Formatting.None
            };

            serializer.Serialize(xmlWriter, target);

            _streaming.OutputContentType = mimeType;
        }

        public T Read<T>()
        {
            var serializer = new XmlSerializer(typeof (T));
            var reader = new StreamReader(_streaming.Input, true);

            return (T) serializer.Deserialize(reader);
        }

        public IEnumerable<string> MatchingMimetypes
        {
            get
            {
                yield return "text/xml";
                yield return "application/xml";
            }
        }
    }
}