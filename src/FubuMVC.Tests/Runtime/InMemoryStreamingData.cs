using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Xml.Serialization;
using FubuMVC.Core.Http;
using HtmlTags;

namespace FubuMVC.Tests.Runtime
{
    public class InMemoryStreamingData : ICurrentHttpRequest
    {
        private Stream _input;

        public void XmlInputIs(object target)
        {
            var serializer = new XmlSerializer(target.GetType());
            MemoryStream stream = new MemoryStream();
            serializer.Serialize(stream, target);
            stream.Position = 0;

            _input = stream;
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


        public string RawUrl()
        {
            throw new NotImplementedException();
        }

        public string RelativeUrl()
        {
            throw new NotImplementedException();
        }

        public string FullUrl()
        {
            throw new NotImplementedException();
        }

        public string ToFullUrl(string url)
        {
            throw new NotImplementedException();
        }

        public string HttpMethod()
        {
            throw new NotImplementedException();
        }

        public bool HasHeader(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetHeader(string key)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> AllHeaderKeys()
        {
            throw new NotImplementedException();
        }

        public NameValueCollection QueryString { get; private set; }

        public Stream Input
        {
            get { return _input; } }

        public bool IsClientConnected()
        {
            throw new NotImplementedException();
        }


        public void CopyOutputToInputForTesting(Stream outputStream)
        {
            _input = outputStream;
            _input.Position = 0;
        }
    }
}