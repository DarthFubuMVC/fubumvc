using System.Runtime.Serialization.Json;
using FubuMVC.Core.Http;

namespace FubuMVC.Core.Behaviors
{
    public class DataContractJsonReader : IJsonReader
    {
        private readonly ICurrentHttpRequest _data;

        public DataContractJsonReader(ICurrentHttpRequest data)
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