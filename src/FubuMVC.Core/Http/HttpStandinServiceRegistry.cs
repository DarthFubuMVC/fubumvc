using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using FubuCore.Binding;
using FubuMVC.Core.Http.Cookies;
using FubuMVC.Core.Http.Headers;
using FubuMVC.Core.Registration;
using FubuMVC.Core.Runtime;

namespace FubuMVC.Core.Http
{
    public class HttpStandInServiceRegistry : ServiceRegistry
    {
        public HttpStandInServiceRegistry()
        {
            SetServiceIfNone<ICurrentHttpRequest, StandInCurrentHttpRequest>();

            SetServiceIfNone<IRequestHeaders, RequestHeaders>();
            SetServiceIfNone<IRequestData>(new RequestData());
            SetServiceIfNone<IClientConnectivity, StandInClientConnectivity>();

            SetServiceIfNone<IHttpWriter, NulloHttpWriter>();

            SetServiceIfNone<ICurrentChain>(new CurrentChain(null, null));
            SetServiceIfNone<IStreamingData, NulloStreamingData>();

            SetServiceIfNone<IResponse, NulloResponse>();
        }

        public class NulloStreamingData : IStreamingData
        {
            public Stream Input
            {
                get { return new MemoryStream(); }
            }
        }

        public class NulloResponse : IResponse
        {
            public int StatusCode
            {
                get { throw new NotImplementedException(); }
            }

            public string StatusDescription
            {
                get { throw new NotImplementedException(); }
            }

            public string HeaderValueFor(HttpResponseHeader key)
            {
                throw new NotImplementedException();
            }

            public string HeaderValueFor(string headerKey)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<Header> AllHeaders()
            {
                throw new NotImplementedException();
            }
        }
    }
}