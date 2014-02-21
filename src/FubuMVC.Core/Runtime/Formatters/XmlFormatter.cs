using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using FubuCore.Descriptions;
using FubuMVC.Core.Http;
using FubuMVC.Core.Resources.Conneg;

namespace FubuMVC.Core.Runtime.Formatters
{
    // See the integration tests for conneg
    [MimeType("text/xml", "application/xml")]
    [Title("Xml Serialization")]
    public class XmlFormatter : IFormatter
    {
        private readonly ICurrentHttpRequest _streaming;
        private readonly IOutputWriter _writer;

        public XmlFormatter(ICurrentHttpRequest streaming, IOutputWriter writer)
        {
            _streaming = streaming;
            _writer = writer;
        }

        public void Write<T>(T target, string mimeType)
        {
            var serializer = new XmlSerializer(typeof (T));
            _writer.Write(mimeType, stream =>
            {
                var xmlWriter = new XmlTextWriter(stream, Encoding.Unicode)
                {
                    Formatting = Formatting.None
                };

                serializer.Serialize(xmlWriter, target);
            });
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