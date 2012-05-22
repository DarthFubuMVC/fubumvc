using System;
using System.IO;
using System.Net;
using System.Web;
using FubuCore;

namespace FubuMVC.Core.Http
{
    public interface IHttpWriter
    {
        void AppendHeader(string key, string value);
        void WriteFile(string file);
        void WriteContentType(string contentType);
        void Write(string content);
        void Redirect(string url);
        void WriteResponseCode(HttpStatusCode status, string description = null);
        void AppendCookie(HttpCookie cookie);

        void Write(Action<Stream> output);
        void Flush();
    }

    // TODO -- flesh this out
    public class RecordingHttpWriter : IHttpWriter
    {
        private readonly StringWriter _writer = new StringWriter();

        public void AppendHeader(string key, string value)
        {
            throw new NotImplementedException();
        }

        public void WriteFile(string file)
        {
            throw new NotImplementedException();
        }

        public void WriteContentType(string contentType)
        {
            throw new NotImplementedException();
        }

        public void Write(string content)
        {
            throw new NotImplementedException();
        }

        public void Redirect(string url)
        {
            throw new NotImplementedException();
        }

        public void WriteResponseCode(HttpStatusCode status, string description = null)
        {
            throw new NotImplementedException();
        }

        public void AppendCookie(HttpCookie cookie)
        {
            throw new NotImplementedException();
        }

        public void Write(Action<Stream> output)
        {
            var stream = new MemoryStream();
            output(stream);

            stream.Position = 0;
            _writer.WriteLine(stream.ReadAllText());
        }

        public void Flush()
        {
            // definitely don't do anything here
        }

        public string AllText()
        {
            return _writer.ToString();
        }
    }

}