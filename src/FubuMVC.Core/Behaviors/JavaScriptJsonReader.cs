using System.Web.Script.Serialization;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Behaviors
{
    public class JavaScriptJsonReader : IJsonReader
    {
        private readonly IStreamingData _data;

        public JavaScriptJsonReader(IStreamingData data)
        {
            _data = data;
        }

        public T Read<T>()
        {
            var serializer = new JavaScriptSerializer();
            string inputText = _data.InputText();
            return serializer.Deserialize<T>(inputText);
        }
    }
}