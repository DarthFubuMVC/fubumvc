using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Rest.Media.Formatters
{
    // See the StoryTeller tests for conneg
    public class XmlFormatter : IFormatter
    {
        private readonly IStreamingData _streaming;

        public XmlFormatter(IStreamingData streaming)
        {
            _streaming = streaming;
        }

        public void Write<T>(T target)
        {
            var serializer = new XmlSerializer(typeof (T));

            // TODO -- later, we'll need to get more sophisticated and worry about the Encoding
            var xmlWriter = new XmlTextWriter(_streaming.Output, Encoding.Default){
                Formatting = Formatting.None
            };

            serializer.Serialize(xmlWriter, target);

            // TODO -- this needs to be connected to the requested mimetype
            _streaming.OutputContentType = MatchingMimetypes.First();
        }

        public T Read<T>()
        {
            var serializer = new XmlSerializer(typeof (T));
            return (T) serializer.Deserialize(_streaming.Input);
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