using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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

        public Task Write(string contentType, string renderedOutput)
        {
            ContentType = contentType;
            _writer.WriteLine(renderedOutput);

            return Task.CompletedTask;
        }

        public Task Write(string renderedOutput)
        {
            _writer.WriteLine(renderedOutput);

            return Task.CompletedTask;
        }

        public string ContentType { get; set; }

        public Task<IRecordedOutput> Record(Func<Task> inner)
        {
            throw new NotImplementedException();
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

        public Task Write(string contentType, Func<Stream, Task> output)
        {
            ContentType = contentType;
            return output(_output);
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