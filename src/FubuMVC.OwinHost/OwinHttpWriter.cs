using System;
using System.IO;
using System.Net;
using System.Security.AccessControl;
using System.Web;
using FubuCore;
using FubuCore.Util;
using FubuMVC.Core.Http;
using Gate.Helpers;

namespace FubuMVC.OwinHost
{
    public class OwinHttpWriter : IHttpWriter
    {
        private readonly Response _response;
        private readonly Cache<string, string> _headers;

        public OwinHttpWriter(Response response)
        {
            _response = response;
            _headers = new Cache<string, string>(response.Headers);
        }

        public void AppendHeader(string key, string value)
        {
            // TODO -- got to watch this one.  Won't work with dup's
            // cookies won't fly
            _headers[key] = value;
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
            _response.ContentType = contentType;
        }

        public void Write(string content)
        {
            _response.Write(content);
        }

        // TODO -- dunno how to do this one off hand
        public void Redirect(string url)
        {
            throw new NotImplementedException();
        }

        public void WriteResponseCode(HttpStatusCode status)
        {
            _response.Status = status.As<int>().ToString();
        }

        public void AppendCookie(HttpCookie cookie)
        {
            throw new NotImplementedException();
        }

        public void Write(Action<Stream> output)
        {
            Action complete = () => { };
            var stream = new OutputStream((segment, continuation) =>
            {
                _response.BinaryWrite(segment);
                return true;
            }, complete);

            output(stream);
        }
    }
}