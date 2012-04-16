using System.Collections.Generic;
using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Resources.Conneg.New;

namespace FubuMVC.Core.Runtime.Formatters
{
    // See the StoryTeller tests for conneg
    [MimeType("application/json", "text/json")]
    public class JsonFormatter : IFormatter
    {
        private readonly IJsonReader _reader;
        private readonly IJsonWriter _writer;

        public JsonFormatter(IJsonWriter writer, IJsonReader reader)
        {
            _writer = writer;
            _reader = reader;
        }

        public void Write<T>(T target, string mimeType)
        {
            _writer.Write(target, mimeType);
        }

        public T Read<T>()
        {
            return _reader.Read<T>();
        }

        public IEnumerable<string> MatchingMimetypes
        {
            get
            {
                yield return "application/json";
                yield return "text/json";
            }
        }
    }
}