using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace FubuMVC.Core.Runtime
{
    public class InMemoryOutputWriter : IOutputWriter
    {
        private readonly StringBuilder _builder = new StringBuilder();
        private readonly StringWriter _writer;

        public InMemoryOutputWriter()
        {
            _writer = new StringWriter(_builder);
        }

        public void WriteFile(string contentType, string localFilePath, string displayName)
        {
        }

        public void Write(string contentType, string renderedOutput)
        {
            _writer.WriteLine(renderedOutput);
        }

        public RecordedOutput Record(Action action)
        {
            action();
            return new RecordedOutput("","");
        }

        public void RedirectToUrl(string url)
        {
        }

        public void AppendCookie(HttpCookie cookie)
        {
        }

        public void WriteResponseCode(HttpStatusCode status)
        {

        }

        public override string ToString()
        {
            return _builder.ToString();
        }
    }
}