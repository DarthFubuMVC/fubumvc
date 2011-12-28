using System;
using System.Collections.Generic;
using System.IO;
using FubuCore;
using FubuMVC.Core.Runtime;
using Environment = Gate.Environment;

namespace FubuMVC.OwinHost
{
    public class OwinRequestBody
    {
        private static readonly char[] CommaSemicolon = new[]{',', ';'};

        private readonly string _contentType;
        private readonly Environment _environment;
        private readonly MemoryStream _stream = new MemoryStream();
        private readonly string _mediaType;

        public OwinRequestBody(Environment environment)
        {
            _environment = environment;
            _contentType = environment.ContentType();

            if (_contentType != null)
            {
                var delimiterPos = _contentType.IndexOfAny(CommaSemicolon);
                _mediaType = delimiterPos < 0 ? _contentType : _contentType.Substring(0, delimiterPos);
            }
        }

        public Environment Environment
        {
            get { return _environment; }
        }

        public bool HasFormData()
        {
            if (_mediaType == MimeType.HttpFormMimetype) return true;
            if (_mediaType == MimeType.MultipartMimetype) return true;

            if (string.Equals(_environment.Method, "POST", StringComparison.OrdinalIgnoreCase)) return true;

            return false;
        }

        public IOwinRequestReader DetermineReader()
        {
            var isFormData = _mediaType == MimeType.HttpFormMimetype || _mediaType == MimeType.MultipartMimetype;

            return isFormData
                       ? (IOwinRequestReader) new FormRequestReader(dict => FormData = dict)
                       : new StreamDataReader(_stream);

        }

        public IDictionary<string, string> FormData { get; private set; }



        public MemoryStream Stream
        {
            get { return _stream; }
        }

        public string PathBase
        {
            get { return _environment.PathBase; }
        }

        public string Path
        {
            get { return _environment.Path; }
        }

        public string HostWithPort
        {
            get
            {
                var headers = Headers();

                string hostHeader = null;
                if (headers.ContainsKey("Host"))
                {
                    hostHeader = headers["Host"];
                }

                if (hostHeader.IsNotEmpty())
                {
                    return hostHeader;
                }

                var serverName = _environment.Get<string>("server.SERVER_NAME");
                if (string.IsNullOrWhiteSpace(serverName))
                    serverName = _environment.Get<string>("server.SERVER_ADDRESS");
                var serverPort = _environment.Get<string>("server.SERVER_PORT");

                return serverName + ":" + serverPort;
            }
        }

        public string Method
        {
            get { return _environment.Method; }
        }


        public Func<ArraySegment<byte>, Action, bool> GetRequestBodyBuilder()
        {
            var reader = DetermineReader();


            return (arraySegments, continuation) =>
            {
                reader.Read(arraySegments.Array, arraySegments.Offset, arraySegments.Count);
                if (continuation == null)
                {
                    reader.Finish();
                    return false;
                }

                continuation();
                return true;
            };
        }

        public IDictionary<string, string> Querystring()
        {
            var querystring = _environment.Get<string>("owin.RequestQueryString");
            return FormRequestReader.Parse(querystring);
        }


        public IDictionary<string, string> Headers()
        {
            return _environment.Headers ?? new Dictionary<string, string>();
        }
    }
}