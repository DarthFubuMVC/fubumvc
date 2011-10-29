using System;
using System.IO;
using System.Xml.Serialization;
using FubuMVC.Core.Http;
using HtmlTags;

namespace FubuMVC.Tests.Runtime
{
    public class InMemoryStreamingData : IStreamingData
    {
        private Stream _input;

        public void XmlInputIs(object target)
        {
            var serializer = new XmlSerializer(target.GetType());
            MemoryStream stream = new MemoryStream();
            serializer.Serialize(stream, target);
            stream.Position = 0;

            _input = stream;
        }

        public void JsonInputIs(object target)
        {
            var json = JsonUtil.ToJson(target);

            JsonInputIs(json);
        }

        public void JsonInputIs(string json)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(json);
            writer.Flush();

            stream.Position = 0;

            _input = stream;
        }




        public Stream Input
        {
            get { return _input; } }


        public void CopyOutputToInputForTesting(Stream outputStream)
        {
            _input = outputStream;
            _input.Position = 0;
        }
    }
}