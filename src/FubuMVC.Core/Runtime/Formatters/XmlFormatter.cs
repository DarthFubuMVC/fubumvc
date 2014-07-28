using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using FubuCore.Descriptions;

namespace FubuMVC.Core.Runtime.Formatters
{
    // See the integration tests for conneg
    [Title("Xml Serialization")]
    [Description("Wrapper around the built in XmlSerializer")]
    public class XmlFormatter : IFormatter
    {
        public void Write<T>(IFubuRequestContext context, T target, string mimeType)
        {
            var serializer = new XmlSerializer(typeof (T));
            context.Writer.Write(mimeType, stream => {
                var xmlWriter = new XmlTextWriter(stream, Encoding.Unicode)
                {
                    Formatting = Formatting.None
                };

                serializer.Serialize(xmlWriter, target);
            });
        }

        public T Read<T>(IFubuRequestContext context)
        {
            var serializer = new XmlSerializer(typeof (T));
            var reader = new StreamReader(context.Request.Input, true);

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