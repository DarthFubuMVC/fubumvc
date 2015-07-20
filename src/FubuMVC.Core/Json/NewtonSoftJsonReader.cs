using System.IO;
using System.Text;
using FubuMVC.Core.Http;

namespace FubuMVC.Core.Json
{
    public class NewtonSoftJsonReader
    {
        private readonly IHttpRequest _request;
        private readonly IJsonSerializer _serializer;

        public NewtonSoftJsonReader(IHttpRequest request, IJsonSerializer serializer)
        {
            _request = request;
            _serializer = serializer;
        }

        public T Read<T>()
        {
            string inputText = GetInputText();
            return _serializer.Deserialize<T>(inputText);
        }

        // Leave this here for testing
        public virtual string GetInputText()
        {
            Encoding encoding = Encoding.UTF8;
            if (_request.HasHeader(HttpGeneralHeaders.ContentEncoding))
            {
                encoding = Encoding.GetEncoding(_request.GetSingleHeader(HttpGeneralHeaders.ContentEncoding));
            }

            if (_request.HasHeader("x-encoding"))
            {
                encoding = Encoding.GetEncoding(_request.GetSingleHeader("x-encoding"));
            }


            var reader = new StreamReader(_request.Input, encoding);

            return reader.ReadToEnd();
        }
    }
}