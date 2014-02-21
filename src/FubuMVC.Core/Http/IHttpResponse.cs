using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using FubuCore;
using FubuMVC.Core.Http.Compression;
using FubuMVC.Core.Http.Headers;

namespace FubuMVC.Core.Http
{
    /// <summary>
    /// Lowest level service to write to the Http output
    /// </summary>
    public interface IHttpResponse
    {
        /// <summary>
        /// Write a header value to the response
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void AppendHeader(string key, string value);

        /// <summary>
        /// Write the contents of the file location out into the output stream
        /// </summary>
        /// <param name="file"></param>
        void WriteFile(string file);

        /// <summary>
        /// Set the Content-Type response header
        /// </summary>
        /// <param name="contentType"></param>
        void WriteContentType(string contentType);

        /// <summary>
        /// Write string content to the output.  This method can be called multiple times
        /// for each request.  Many Bothans died to bring you this functionality.
        /// </summary>
        /// <param name="content"></param>
        void Write(string content);

        /// <summary>
        /// Writes a 302 redirect response and body to the http output
        /// </summary>
        /// <param name="url"></param>
        void Redirect(string url);

        /// <summary>
        /// Writes the status code and an optional status description to the response
        /// </summary>
        /// <param name="status"></param>
        /// <param name="description"></param>
        void WriteResponseCode(HttpStatusCode status, string description = null);

        /// <summary>
        /// The HTTP status code.  The default is 200
        /// </summary>
        int StatusCode { get; set; }

        /// <summary>
        /// The HTTP status description header
        /// </summary>
        string StatusDescription { get; set; }


        /// <summary>
        /// All response header values for the key
        /// </summary>
        /// <param name="headerKey"></param>
        /// <returns></returns>
        IEnumerable<string> HeaderValueFor(string headerKey);

        /// <summary>
        /// All appended Header values, per key and value
        /// </summary>
        /// <returns></returns>
        IEnumerable<Header> AllHeaders();

        /// <summary>
        /// Applies output encoding to the response.  Mostly used by FubuMVC's internal support for content compression
        /// </summary>
        /// <param name="encoding"></param>
        void UseEncoding(IHttpContentEncoding encoding);

        /// <summary>
        /// Write directly to the output stream
        /// </summary>
        /// <param name="output"></param>
        void Write(Action<Stream> output);

        /// <summary>
        /// Force the output to write to the http output.  Do NOT use this unless you know exactly what the ramifications are, and
        /// this varies by host type.  
        /// </summary>
        void Flush();
    }

    // TODO -- flesh this out
    public class RecordingHttpResponse : IHttpResponse
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

        public int StatusCode { get; set; }
        public string StatusDescription { get; set; }

        public IEnumerable<string> HeaderValueFor(string headerKey)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Header> AllHeaders()
        {
            throw new NotImplementedException();
        }

        public void UseEncoding(IHttpContentEncoding encoding)
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