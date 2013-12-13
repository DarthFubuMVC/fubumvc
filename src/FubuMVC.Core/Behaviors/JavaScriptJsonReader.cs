using System.Web.Script.Serialization;

using FubuMVC.Core.Http;


namespace FubuMVC.Core.Behaviors
{
    public class JavaScriptJsonReader : IJsonReader
    {
        private readonly IStreamingData _data;
        private readonly JavaScriptSerializer _serializer;

        public JavaScriptJsonReader(IStreamingData data)
        {
            _data = data;
            _serializer = new JavaScriptSerializer();
        }

        public T Read<T>()
        {
            string inputText = _data.InputText();
            return _serializer.Deserialize<T>(inputText);
        }
    }
}