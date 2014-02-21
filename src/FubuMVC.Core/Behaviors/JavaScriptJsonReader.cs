using System.Web.Script.Serialization;
using FubuMVC.Core.Http;


namespace FubuMVC.Core.Behaviors
{
    public class JavaScriptJsonReader : IJsonReader
    {
        private readonly ICurrentHttpRequest _data;
        private readonly JavaScriptSerializer _serializer;

        public JavaScriptJsonReader(ICurrentHttpRequest data)
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