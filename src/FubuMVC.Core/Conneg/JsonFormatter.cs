using FubuMVC.Core.Behaviors;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Conneg
{
    // See the StoryTeller tests for conneg
    public class JsonFormatter : IFormatter
    {
        private readonly IJsonWriter _writer;
        private readonly IJsonReader _reader;

        public JsonFormatter(IJsonWriter writer, IJsonReader reader)
        {
            _writer = writer;
            _reader = reader;
        }

        public T Read<T>()
        {
            return _reader.Read<T>();
        }

        public void Write<T>(T target)
        {
            _writer.Write(target);
        }

        public bool Matches(CurrentRequest request)
        {
            return request.MatchesOneOfTheseMimeTypes("application/json", "text/json");
        }
    }
}