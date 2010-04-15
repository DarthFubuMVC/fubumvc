using System.Diagnostics;
using System.IO;
using HtmlTags;

namespace FubuMVC.Core.Runtime
{
    public class InMemoryStreamingData : IStreamingData
    {
        private Stream _input;
        private Stream _output;

        public InMemoryStreamingData()
        {
            _output = new MemoryStream();
        }

        public void RewindOutput()
        {
            _output.Position = 0;
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

        public Stream Output
        {
            get { return _output; } }
    }
}