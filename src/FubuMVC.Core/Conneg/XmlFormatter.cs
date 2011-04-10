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

        public T Read<T>()
        {
            var serializer = new XmlSerializer(typeof (T));
            return (T) serializer.Deserialize(_streaming.Input);
        }

        public void Write<T>(T target)
        {
            var serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(_streaming.Output, target);
        }

        public bool Matches(CurrentRequest request)
        {
            return request.MatchesOneOfTheseMimeTypes("text/xml", "application/xml");
        }
    }
}