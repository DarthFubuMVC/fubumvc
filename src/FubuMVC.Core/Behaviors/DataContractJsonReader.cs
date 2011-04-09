using System.Runtime.Serialization.Json;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Behaviors
{
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
}