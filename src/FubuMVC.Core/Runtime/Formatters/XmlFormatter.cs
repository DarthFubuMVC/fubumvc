using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
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
        public Task Write<T>(IFubuRequestContext context, T target, string mimeType)
        {
            var serializer = new XmlSerializer(typeof (T));
            return context.Writer.Write(mimeType, async stream =>
            {
                var stringWriter = new StringWriter();

                var xmlWriter = new XmlTextWriter(stringWriter)
                {
                    Formatting = Formatting.None,
                };


                serializer.Serialize(xmlWriter, target);

                var writer = new StreamWriter(stream) {AutoFlush = true};

                await writer.WriteAsync(stringWriter.ToString()).ConfigureAwait(false);
            });
        }

        public async Task<T> Read<T>(IFubuRequestContext context)
        {
            var serializer = new XmlSerializer(typeof (T));
            var reader = new StreamReader(context.Request.Input, true);

            var xml = await reader.ReadToEndAsync().ConfigureAwait(false);

            return (T) serializer.Deserialize(new XmlTextReader(new StringReader(xml)));
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