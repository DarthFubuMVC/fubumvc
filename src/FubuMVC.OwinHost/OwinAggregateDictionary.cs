using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Routing;
using FubuCore;
using FubuCore.Binding;
using FubuMVC.Core.Http;
using FubuMVC.Core.Runtime;
using Environment = Gate.Environment;

namespace FubuMVC.OwinHost
{
    public static class EnvironmentExtensions
    {
        public static string ContentType(this Environment environment)
        {
            string value;
            return (environment.Headers != null && environment.Headers.TryGetValue("Content-Type", out value))
                       ? value
                       : null;
        }

        public static T Get<T>(this Environment environment, string name)
        {
            object value;
            return environment.TryGetValue(name, out value) ? (T)value : default(T);
        }
    }

    public class OwinRequestBody
    {
        private static readonly char[] CommaSemicolon = new[]{',', ';'};

        private readonly string _contentType;
        private readonly Environment _environment;
        private readonly object _locker = new object();
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

    public interface IOwinRequestReader
    {
        void Read(byte[] bytes, int offset, int count);
        void Finish();
    }

    public class FormRequestReader : IOwinRequestReader
    {
        private readonly Action<IDictionary<string, string>> _finish;
        private readonly StringBuilder _builder;
        private readonly Encoding _encoding;

        public FormRequestReader(Action<IDictionary<string, string>> finish)
        {
            _finish = finish;
            _builder = new StringBuilder();
            _encoding = Encoding.UTF8; // TODO -- is this a good idea?
        }

        public void Read(byte[] bytes, int offset, int count)
        {
            var text = _encoding.GetString(bytes, offset, count);
            _builder.Append(text);
        }

        public IDictionary<string, string> PostData()
        {
            return Parse(_builder.ToString());
        }

        public static IDictionary<string, string> Parse(string queryString)
        {
            // TODO: this is wrong in many, many ways
            return (queryString ?? "").Split("&".ToCharArray())
                .Select(item => item.Split("=".ToCharArray(), 2))
                .Where(item => item.Length == 2)
                .GroupBy(item => item[0], item => Decode(item[1]), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => string.Join(",", g.ToArray()), StringComparer.OrdinalIgnoreCase);

        }

        static string Decode(string value)
        {
            return value.Replace("%3A", ":").Replace("%2F", "/");
        }

        public void Finish()
        {
            _finish(PostData());
        }
    }

    public class StreamDataReader : IOwinRequestReader
    {
        private readonly MemoryStream _stream;

        public StreamDataReader(MemoryStream stream)
        {
            _stream = stream;
        }

        public void Read(byte[] bytes, int offset, int count)
        {
            _stream.Write(bytes, offset, count);
        }

        public void Finish()
        {
            
        }
    }

    public class OwinAggregateDictionary : AggregateDictionary
    {
        public OwinAggregateDictionary(RouteData routeData, OwinRequestBody body)
        {
            // TODO -- this is duplication w/ AspNetAggregateDictionary.  DRY it baby!
            Func<string, object> locator = key =>
            {
                object found;
                routeData.Values.TryGetValue(key, out found);
                return found;
            };


            AddLocator(RequestDataSource.Route.ToString(), locator, () => routeData.Values.Keys);

            addDictionaryLocator("Query string", body.Querystring());
            addDictionaryLocator("Form Post", body.FormData ?? new Dictionary<string, string>());

            addDictionaryLocator(RequestDataSource.Header.ToString(), body.Headers());
        }

        private void addDictionaryLocator(string name, IDictionary<string, string> dictionary)
        {
            Func<string, object> locator = key => dictionary.ContainsKey(key) ? dictionary[key] : null;

            AddLocator(name, locator, () => dictionary.Keys);
        }
    }

}