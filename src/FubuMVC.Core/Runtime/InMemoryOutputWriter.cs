using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using FubuCore.Util;
using FubuMVC.Core.Caching;
using Cookie = FubuMVC.Core.Http.Cookies.Cookie;

namespace FubuMVC.Core.Runtime
{
    public class InMemoryOutputWriter : IOutputWriter
    {
        private readonly StringBuilder _builder = new StringBuilder();
        private readonly StringWriter _writer;
        private readonly MemoryStream _output = new MemoryStream();
        private readonly Cache<string, string> _headers = new Cache<string, string>();

        public InMemoryOutputWriter()
        {
            _writer = new StringWriter(_builder);
        }

        public void WriteFile(string contentType, string localFilePath, string displayName)
        {
        }

        public void Write(string contentType, string renderedOutput)
        {
            ContentType = contentType;
            _writer.WriteLine(renderedOutput);
        }

        public void Write(string renderedOutput)
        {
            _writer.WriteLine(renderedOutput);
        }

        public string ContentType { get; set; }

        public IRecordedOutput Record(Action action)
        {
            throw new NotImplementedException("isn't really built");
            //action();
            //return new RecordedOutput(null);
        }

        public void Replay(IRecordedOutput output)
        {
            throw new NotImplementedException();
        }

        public void Flush()
        {
        }

        public void RedirectToUrl(string url)
        {
        }

        public void AppendCookie(Cookie cookie)
        {
        }

        public void AppendHeader(string key, string value)
        {
            _headers[key] = value;
        }

        public Cache<string, string> Headers
        {
            get { return _headers; }
        }

        public void Write(string contentType, Action<Stream> output)
        {
            ContentType = contentType;
            output(_output);
        }

        public void WriteResponseCode(HttpStatusCode status, string description = null)
        {
        }

        public override string ToString()
        {
            return _builder.ToString();
        }

        public Stream OutputStream()
        {
            _output.Position = 0;
            return _output;
        }
    }
}