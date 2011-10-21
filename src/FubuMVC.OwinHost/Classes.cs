using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web;
using System.Web.Routing;
using FubuCore;
using FubuCore.Binding;
using FubuCore.Util;
using FubuMVC.Core.Http;
using Gate;
using Gate.Helpers;

namespace FubuMVC.OwinHost
{
    public class OwinServiceArguments : ServiceArguments
    {
        public OwinServiceArguments(RouteData routeData, IDictionary<string, object> env, ResultDelegate result)
        {
            var request = new Request(env);
            var response = new Response(result);


            With<AggregateDictionary>(new OwinAggregateDictionary(routeData, request));

            With<ICurrentRequest>(new OwinCurrentRequest(request));
            With<IStreamingData>(new OwinStreamingData(request, response));
            With<IHttpWriter>(new OwinHttpWriter(response));
        }
    }

    public class OwinAggregateDictionary : AggregateDictionary
    {
        public OwinAggregateDictionary(RouteData routeData, Request request)
        {
            // TODO -- this is duplication w/ AspNetAggregateDictionary.  DRY it baby!
            Func<string, object> locator = key =>
            {
                object found;
                routeData.Values.TryGetValue(key, out found);
                return found;
            };


            AddLocator(RequestDataSource.Route, locator, () => routeData.Values.Keys);

            addDictionaryLocator("Query string", request.Query);
            addDictionaryLocator("Form Post", request.Post);

            addDictionaryLocator(RequestDataSource.Header.ToString(), request.Headers);

            // TODO -- files?
        }

        private void addDictionaryLocator(string name, IDictionary<string, string> dictionary)
        {
            Func<string, object> locator = key => { return dictionary.ContainsKey(key) ? dictionary[key] : null; };

            AddLocator(name, locator, () => dictionary.Keys);
        }
    }

    public class OwinCurrentRequest : ICurrentRequest
    {
        private readonly Request _request;

        public OwinCurrentRequest(Request request)
        {
            _request = request;
        }

        public string RawUrl()
        {
            return _request.Path.ToAbsoluteUrl(_request.PathBase);
        }

        public string RelativeUrl()
        {
            return _request.Path;
        }

        public string ApplicationRoot()
        {
            return _request.PathBase;
        }

        public string HttpMethod()
        {
            return _request.Method;
        }
    }

    public class OwinStreamingData : IStreamingData
    {
        private readonly Request _request;
        private readonly Response _response;

        public OwinStreamingData(Request request, Response response)
        {
            _request = request;
            _response = response;
        }

        public Stream Input
        {
            get { return new InputStream(_request.Body); }
        }

        public Stream Output
        {
            get
            {
                Action complete = () => { };
                return new OutputStream((segment, continuation) =>
                {
                    _response.BinaryWrite(segment);
                    return true;
                }, complete);
            }
        }

        public string OutputContentType
        {
            get { return _response.ContentType; }
            set { _response.ContentType = value; }
        }
    }

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
            throw new NotImplementedException();
        }

        public void WriteContentType(string contentType)
        {
            _response.ContentType = contentType;
        }

        public void Write(string content)
        {
            _response.Write(content);
        }

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
    }
}