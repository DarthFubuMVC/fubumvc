using System;
using System.Runtime.Serialization.Json;
using System.Web.Script.Serialization;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Behaviors
{
    public class DeserializeJsonBehavior<T> : BasicBehavior where T : class
    {
        private readonly IJsonReader _reader;
        private readonly IFubuRequest _request;

        public DeserializeJsonBehavior(IJsonReader reader, IFubuRequest request) : base(PartialBehavior.Executes)
        {
            _reader = reader;
            _request = request;
        }

        protected override DoNext performInvoke()
        {
            var input = _reader.Read<T>();
            _request.Set(input);

            return DoNext.Continue;
        }
    }

    public interface IJsonReader
    {
        T Read<T>();
    }

    public class DataContractJsonReader : IJsonReader
    {
        private readonly IStreamingData _data;

        public DataContractJsonReader(IStreamingData data)
        {
            _data = data;
        }

        public T Read<T>()
        {
            var serializer = new DataContractJsonSerializer(typeof (T));
            return (T) serializer.ReadObject(_data.Input);
        }
    }

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