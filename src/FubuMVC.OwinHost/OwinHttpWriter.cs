using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using FubuCore;
using FubuMVC.Core.Http;
using FubuMVC.Core.Http.Compression;

namespace FubuMVC.OwinHost
{
    public class OwinHttpWriter : IHttpWriter, IDisposable
    {
        private readonly IDictionary<string, object> _environment;
        private readonly MemoryStream _output;

        public OwinHttpWriter(IDictionary<string, object> environment)
        {
            _environment = environment;

            _output = new MemoryStream();
        }

        public void AppendHeader(string key, string value)
        {
            var headers = _environment.Get<IDictionary<string, string[]>>(OwinConstants.ResponseHeadersKey);
            headers[key] = new[] {value};
        }

        public void WriteFile(string file)
        {
            using (var fileStream = new FileStream(file, FileMode.Open))
            {
                Write(stream => fileStream.CopyTo(stream, 64000));
            }
        }

        public void WriteContentType(string contentType)
        {
            AppendHeader(HttpResponseHeaders.ContentType, contentType);
        }

        public void Write(string content)
        {
            var writer = new StreamWriter(_output){AutoFlush = true};
            writer.Write(content);
        }

        public void Redirect(string url)
        {
            // TODO: This is a hack, better way to accomplish this?
            _environment[OwinConstants.ResponseStatusCodeKey] = HttpStatusCode.Redirect;
            AppendHeader("Location", url);
            Write(
                string.Format(
                    "<html><head><title>302 Found</title></head><body><h1>Found</h1><p>The document has moved <a href='{0}'>here</a>.</p></body></html>",
                    url));
        }

        public void WriteResponseCode(HttpStatusCode status, string description = null)
        {
            _environment[OwinConstants.ResponseStatusCodeKey] = status.As<int>();
            _environment[OwinConstants.ResponseReasonPhraseKey] = description;
        }

        public void UseEncoding(IHttpContentEncoding encoding)
        {
            // TODO -- Come back to this one. The integration tests can't be done until we do
        }

        public void Write(Action<Stream> output)
        {
            output(_output);
        }

        public void Flush()
        {
            var response = _environment.Get<Stream>(OwinConstants.ResponseBodyKey);
            _output.Position =0;
            _output.CopyTo(response);
        }

        public void Dispose()
        {
            Flush();
        }
    }
}